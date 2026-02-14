import { createRouter, createWebHistory } from 'vue-router';
import HomeView from '../views/HomeView.vue';
import AdminDashboard from '../views/AdminDashboard.vue';
import ShopView from '../views/ShopView.vue';
import ProductDetailView from '../views/ProductDetailView.vue';
import LoginView from '../views/LoginView.vue';
import UserProfileView from '../views/UserProfileView.vue';
import { useAuthStore } from '../stores/auth';

const router = createRouter({
    history: createWebHistory(import.meta.env.BASE_URL),
    routes: [
        {
            path: '/',
            name: 'home',
            component: HomeView
        },
        {
            path: '/shop',
            name: 'shop',
            component: ShopView
        },
        {
            path: '/product/:slug',
            name: 'product-detail',
            component: ProductDetailView
        },
        {
            path: '/login',
            name: 'login',
            component: LoginView
        },
        {
            path: '/register',
            name: 'register',
            component: () => import('../views/RegisterView.vue')
        },
        {
            path: '/profile',
            name: 'profile',
            component: UserProfileView,
            meta: { requiresAuth: true }
        },
        {
            path: '/admin',
            name: 'admin',
            component: AdminDashboard,
            meta: { requiresAdmin: true }
        }
    ],
    scrollBehavior(to, from, savedPosition) {
        // Use params to avoid TS error
        void to;
        void from;

        if (savedPosition) {
            return savedPosition;
        } else {
            return { top: 0, behavior: 'smooth' };
        }
    }
});

// Navigation Guard
router.beforeEach((to, from, next) => {
    void from;
    const authStore = useAuthStore();

    // Check for Auth Requirement
    if (to.meta.requiresAuth && !authStore.isAuthenticated) {
        next('/login');
        return;
    }

    // Check for Admin Requirement
    if (to.meta.requiresAdmin && (!authStore.isAuthenticated || (authStore.user as any)?.role !== 'Admin')) {
        next('/');
        return;
    }

    next();
});

export default router;
