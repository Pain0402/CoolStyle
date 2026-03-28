import axios from 'axios';

const apiClient = axios.create({
    baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5058/api',
    timeout: 10000,
    headers: { 'Content-Type': 'application/json' },
});

// Track if a refresh is already in progress to avoid race conditions
let isRefreshing = false;
let failedQueue: Array<{ resolve: (value: any) => void; reject: (reason?: any) => void }> = [];

const processQueue = (error: any, token: string | null = null) => {
    failedQueue.forEach(({ resolve, reject }) => {
        if (error) reject(error);
        else resolve(token);
    });
    failedQueue = [];
};

// Response Interceptor
apiClient.interceptors.response.use(
    (response) => {
        // Unwrap JSend envelope
        const res = response.data;
        if (res.status === 'success') return res.data;
        return Promise.reject(new Error(res.message || 'Unknown Error'));
    },
    async (error) => {
        const originalRequest = error.config;

        // Handle 401 - attempt token refresh
        if (error.response?.status === 401 && !originalRequest._retry) {
            const refreshToken = localStorage.getItem('refreshToken');

            // No refresh token → logout immediately
            if (!refreshToken) {
                clearAuthAndRedirect();
                return Promise.reject(error);
            }

            if (isRefreshing) {
                // Queue this request until refresh completes
                return new Promise((resolve, reject) => {
                    failedQueue.push({ resolve, reject });
                }).then((token) => {
                    originalRequest.headers['Authorization'] = `Bearer ${token}`;
                    return apiClient(originalRequest);
                });
            }

            originalRequest._retry = true;
            isRefreshing = true;

            try {
                const response = await axios.post(
                    `${apiClient.defaults.baseURL}/auth/refresh-token`,
                    { refreshToken }
                );

                const data = response.data?.data;
                if (!data?.token) throw new Error('Invalid refresh response');

                // Persist new tokens
                localStorage.setItem('token', data.token);
                localStorage.setItem('refreshToken', data.refreshToken);

                // Update axios default header
                apiClient.defaults.headers.common['Authorization'] = `Bearer ${data.token}`;
                originalRequest.headers['Authorization'] = `Bearer ${data.token}`;

                // Update user store if role changed
                const stored = localStorage.getItem('user');
                if (stored) {
                    const user = JSON.parse(stored);
                    user.role = data.role;
                    localStorage.setItem('user', JSON.stringify(user));
                }

                processQueue(null, data.token);
                return apiClient(originalRequest);
            } catch (refreshError) {
                processQueue(refreshError, null);
                clearAuthAndRedirect();
                return Promise.reject(refreshError);
            } finally {
                isRefreshing = false;
            }
        }

        // Handle other errors
        const status = error.response?.status;
        const message = error.response?.data?.message || error.message || 'Something went wrong';

        if (status === 403) {
            const { useToast } = await import('vue-toastification');
            useToast().error('Bạn không có quyền thực hiện thao tác này.');
        } else if (status === 500) {
            const { useToast } = await import('vue-toastification');
            useToast().error('Lỗi hệ thống, vui lòng thử lại sau.');
        } else if (!error.response && !isRefreshing) {
            // Only show network error once, not for every failed request
            const { useToast } = await import('vue-toastification');
            useToast().error('Không thể kết nối đến server.', { timeout: 3000, id: 'network-error' });
        }

        console.error('[API Error]:', message);
        return Promise.reject(error);
    }
);

function clearAuthAndRedirect() {
    localStorage.removeItem('token');
    localStorage.removeItem('refreshToken');
    localStorage.removeItem('user');
    delete apiClient.defaults.headers.common['Authorization'];
    // Redirect to login if not already there
    if (window.location.pathname !== '/login') {
        window.location.href = '/login';
    }
}

export default apiClient;
