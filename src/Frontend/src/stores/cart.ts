import { defineStore } from 'pinia';
import { ref, computed, watch } from 'vue';

export interface CartItem {
    id: string; // Composite key: productId_sku
    productId: number;
    name: string;
    price: number;
    thumbnailUrl: string;
    quantity: number;
    size: string;
    color: string;
    slug: string;
    sku: string;
}

export const useCartStore = defineStore('cart', () => {
    // Load from LocalStorage if available
    const savedCart = localStorage.getItem('cart');
    const items = ref<CartItem[]>(savedCart ? JSON.parse(savedCart) : []);

    // Persist to LocalStorage whenever items change
    watch(items, (newItems) => {
        localStorage.setItem('cart', JSON.stringify(newItems));
    }, { deep: true });

    const totalItems = computed(() => items.value.reduce((total, item) => total + item.quantity, 0));

    const totalPrice = computed(() => items.value.reduce((total, item) => total + (item.price * item.quantity), 0));

    const addToCart = (product: any, quantity: number = 1, variant: any = null) => {
        // Default values if no variant selected (fallback)
        const size = variant?.size || 'Free Size';
        const color = variant?.colorName || 'Default';
        const sku = variant?.sku || `P${product.id}-DEFAULT`;
        const price = variant?.price || product.basePrice;

        const compositeKey = `${product.id}_${sku}`;

        const existingItem = items.value.find(item => item.id === compositeKey);

        if (existingItem) {
            existingItem.quantity += quantity;
        } else {
            items.value.push({
                id: compositeKey,
                productId: product.id,
                name: product.name,
                price: price,
                thumbnailUrl: product.thumbnailUrl,
                quantity: quantity,
                slug: product.slug,
                size: size,
                color: color,
                sku: sku
            });
        }
    };

    const removeFromCart = (itemId: string) => {
        const index = items.value.findIndex(item => item.id === itemId);
        if (index > -1) {
            items.value.splice(index, 1);
        }
    };

    const updateQuantity = (itemId: string, quantity: number) => {
        const item = items.value.find(item => item.id === itemId);
        if (item) {
            if (quantity <= 0) {
                removeFromCart(itemId);
            } else {
                item.quantity = quantity;
            }
        }
    };

    const clearCart = () => {
        items.value = [];
    };

    return { items, totalItems, totalPrice, addToCart, removeFromCart, updateQuantity, clearCart };
});
