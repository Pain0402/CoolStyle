import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import apiClient from '../utils/api';

export interface User {
    email: string;
    fullName: string;
}

export const useAuthStore = defineStore('auth', () => {
    const user = ref<User | null>(null);
    const token = ref<string | null>(localStorage.getItem('token'));

    const isAuthenticated = computed(() => !!token.value);

    // If we have a token on startup, we should try to restore axios header. 
    // Ideally we would verify token validity too, but this is simple MVP.
    if (token.value) {
        apiClient.defaults.headers.common['Authorization'] = `Bearer ${token.value}`;
        // Note: We don't have user details persisted in localStorage in this simple version, 
        // so user might be null even if token exists. 
        // Improvement: Persist user object or fetch profile on init.
    }

    const setAuth = (newToken: string, newUser: User) => {
        token.value = newToken;
        user.value = newUser;
        localStorage.setItem('token', newToken);

        // Set axios header
        apiClient.defaults.headers.common['Authorization'] = `Bearer ${newToken}`;
    };

    const logout = () => {
        token.value = null;
        user.value = null;
        localStorage.removeItem('token');
        delete apiClient.defaults.headers.common['Authorization'];
    };

    const register = async (payload: any) => {
        const response: any = await apiClient.post('/auth/register', payload);
        // Explicitly cast or access properties known to exist in response
        setAuth(response.token, { email: response.email, fullName: response.fullName });
        return true;
    };

    const login = async (payload: any) => {
        const response: any = await apiClient.post('/auth/login', payload);
        setAuth(response.token, { email: response.email, fullName: response.fullName });
        return true;
    };

    return { user, token, isAuthenticated, login, register, logout };
});
