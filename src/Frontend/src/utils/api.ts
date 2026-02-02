import axios from 'axios';

// Create Axios Instance
const apiClient = axios.create({
    baseURL: import.meta.env.VITE_API_URL || 'http://localhost:5058/api', // Backend URL
    timeout: 10000,
    headers: {
        'Content-Type': 'application/json',
    },
});

// Response Interceptor (Unwrap JSend)
apiClient.interceptors.response.use(
    (response) => {
        // If JSend success, return data directly
        const res = response.data;
        if (res.status === 'success') {
            return res.data;
        }
        return Promise.reject(new Error(res.message || 'Unknown Error'));
    },
    (error) => {
        // Determine Error Message
        let message = 'Something went wrong';
        if (error.response && error.response.data) {
            // Try to read JSend error message
            message = error.response.data.message || error.message;
        }
        console.error('[API Error]:', message);
        return Promise.reject(error);
    }
);

export default apiClient;
