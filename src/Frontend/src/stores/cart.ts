import { defineStore } from 'pinia';
import { ref, computed, watch } from 'vue';

export interface CartItem {
    productId: number;
    name: string;
    price: number;
    thumbnailUrl: string;
    quantity: number;
    size?: string;
    color?: string;
    slug: string;
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

    const addToCart = (product: any, quantity: number = 1) => {
        const existingItem = items.value.find(item => item.productId === product.id);

        if (existingItem) {
            existingItem.quantity += quantity;
        } else {
            items.value.push({
                productId: product.id,
                name: product.name,
                price: product.basePrice,
                thumbnailUrl: product.thumbnailUrl,
                quantity: quantity,
                slug: product.slug,
                // Default variant for MVP (can be extended)
                size: 'L',
                color: 'Default'
            });
        }
    };

    const removeFromCart = (productId: number) => {
        const index = items.value.findIndex(item => item.productId === productId);
        if (index > -1) {
            items.value.splice(index, 1);
        }
    };

    const updateQuantity = (productId: number, quantity: number) => {
        const item = items.value.find(item => item.productId === productId);
        if (item) {
            if (quantity <= 0) {
                removeFromCart(productId);
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
