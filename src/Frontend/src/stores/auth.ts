import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import apiClient from '../utils/api';

export interface User {
    email: string;
    fullName: string;
    avatarUrl?: string;
    role?: string;
}

export const useAuthStore = defineStore('auth', () => {
    const user = ref<User | null>(localStorage.getItem('user') ? JSON.parse(localStorage.getItem('user')!) : null);
    const token = ref<string | null>(localStorage.getItem('token'));

    const isAuthenticated = computed(() => !!token.value);

    // Restore axios header on startup
    if (token.value) {
        apiClient.defaults.headers.common['Authorization'] = `Bearer ${token.value}`;
    }

    const setAuth = (newToken: string, newUser: User, newRefreshToken?: string) => {
        token.value = newToken;
        user.value = newUser;
        localStorage.setItem('token', newToken);
        localStorage.setItem('user', JSON.stringify(newUser));
        if (newRefreshToken) {
            localStorage.setItem('refreshToken', newRefreshToken);
        }
        apiClient.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
    };

    const logout = async () => {
        // Revoke refresh token on server
        const refreshToken = localStorage.getItem('refreshToken');
        if (refreshToken) {
            try {
                await apiClient.post('/auth/revoke-token', { refreshToken });
            } catch {
                // Ignore errors on revoke — still clear local state
            }
        }

        token.value = null;
        user.value = null;
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        localStorage.removeItem('user');
        delete apiClient.defaults.headers.common['Authorization'];
    };

    const register = async (payload: any) => {
        const response: any = await apiClient.post('/auth/register', payload);
        setAuth(
            response.token,
            { email: response.email, fullName: response.fullName, avatarUrl: response.avatarUrl, role: response.role },
            response.refreshToken
        );
        return true;
    };

    const login = async (payload: any) => {
        const response: any = await apiClient.post('/auth/login', payload);
        setAuth(
            response.token,
            { email: response.email, fullName: response.fullName, avatarUrl: response.avatarUrl, role: response.role },
            response.refreshToken
        );
        return true;
    };

    const updateUser = (newUser: User) => {
        user.value = { ...user.value, ...newUser };
        localStorage.setItem('user', JSON.stringify(user.value));
    };

    return { user, token, isAuthenticated, login, register, logout, updateUser, setAuth };
});
