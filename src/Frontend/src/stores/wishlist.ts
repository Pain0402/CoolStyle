import { defineStore } from 'pinia';
import { ref, computed } from 'vue';
import apiClient from '../utils/api';
import { useAuthStore } from './auth';

export const useWishlistStore = defineStore('wishlist', () => {
    const items = ref<any[]>([]);
    const loading = ref(false);
    const authStore = useAuthStore();

    const wishlistIds = computed(() => items.value.map(item => item.productId));

    const fetchWishlist = async () => {
        if (!authStore.isAuthenticated) return;
        try {
            loading.value = true;
            const res: any = await apiClient.get('/wishlist');
            items.value = res || [];
        } catch (e) {
            console.error("Failed to fetch wishlist", e);
        } finally {
            loading.value = false;
        }
    };

    const toggleWishlist = async (product: any) => {
        if (!authStore.isAuthenticated) return false;

        const isItemInWishlist = wishlistIds.value.includes(product.id);

        try {
            if (isItemInWishlist) {
                await apiClient.delete(`/wishlist/${product.id}`);
                items.value = items.value.filter(i => i.productId !== product.id);
                return false;
            } else {
                await apiClient.post(`/wishlist/${product.id}`);
                // Optimistic update or just fetch again
                // For now, let's just add it locally to be fast
                items.value.push({
                    productId: product.id,
                    product: product,
                    addedAt: new Date().toISOString()
                });
                return true;
            }
        } catch (e) {
            console.error("Wishlist toggle error", e);
            return isItemInWishlist; // revert
        }
    };

    const isInWishlist = (productId: number) => {
        return wishlistIds.value.includes(productId);
    };

    return {
        items,
        loading,
        wishlistIds,
        fetchWishlist,
        toggleWishlist,
        isInWishlist
    };
});
