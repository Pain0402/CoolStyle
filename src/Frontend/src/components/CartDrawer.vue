<script setup lang="ts">
import { ref } from 'vue';
import { useCartStore } from '../stores/cart';
import { X, Trash2, Plus, Minus, ShoppingBag } from 'lucide-vue-next';
import CheckoutModal from './CheckoutModal.vue';

const cartStore = useCartStore();
const emit = defineEmits(['close']);
const showCheckout = ref(false);

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

const handleCheckoutSuccess = () => {
    showCheckout.value = false;
    emit('close'); // Close Cart Drawer
};
</script>

<template>
  <div class="fixed inset-0 z-50 overflow-hidden">
    <!-- Backdrop -->
    <div class="absolute inset-0 bg-black/50 backdrop-blur-sm transition-opacity opacity-100" @click="emit('close')"></div>

    <!-- Drawer -->
    <div class="absolute inset-y-0 right-0 max-w-md w-full flex">
      <div class="h-full flex flex-col bg-white shadow-xl animate-slide-in-right w-full">
        
        <!-- Header -->
        <div class="flex items-center justify-between px-6 py-4 border-b">
          <div class="flex items-center gap-2">
             <ShoppingBag :size="20" />
             <h2 class="text-lg font-bold">Giỏ hàng ({{ cartStore.totalItems }})</h2>
          </div>
          <button @click="emit('close')" class="text-gray-400 hover:text-black transition">
            <X :size="24" />
          </button>
        </div>

        <!-- Scrollable Content -->
        <div class="flex-1 overflow-y-auto p-6 space-y-6">
            <div v-if="cartStore.items.length === 0" class="text-center py-20 text-gray-500">
                <ShoppingBag :size="48" class="mx-auto mb-4 opacity-20" />
                <p>Giỏ hàng của bạn đang trống.</p>
                <button @click="emit('close')" class="mt-4 text-blue-600 font-medium hover:underline">Tiếp tục mua sắm</button>
            </div>

            <div v-else class="space-y-6">
                <div v-for="item in cartStore.items" :key="item.productId" class="flex gap-4">
                    <!-- Image -->
                    <div class="h-24 w-20 flex-shrink-0 overflow-hidden rounded-md border border-gray-200">
                        <img :src="item.thumbnailUrl || 'https://placehold.co/100'" :alt="item.name" class="h-full w-full object-cover object-center" />
                    </div>

                    <!-- Details -->
                    <div class="flex flex-1 flex-col">
                        <div>
                            <div class="flex justify-between text-base font-medium text-gray-900">
                                <h3 class="line-clamp-2"><a :href="`/product/${item.slug}`">{{ item.name }}</a></h3>
                                <p class="ml-4">{{ formatCurrency(item.price * item.quantity) }}</p>
                            </div>
                            <p class="mt-1 text-sm text-gray-500">Size: {{ item.size }} | Màu: {{ item.color }}</p>
                        </div>
                        <div class="flex flex-1 items-end justify-between text-sm">
                            <div class="flex items-center border rounded-md">
                                <button @click="cartStore.updateQuantity(item.productId, item.quantity - 1)" class="p-1 hover:bg-gray-100 text-gray-600"><Minus :size="16"/></button>
                                <span class="px-2 font-medium">{{ item.quantity }}</span>
                                <button @click="cartStore.updateQuantity(item.productId, item.quantity + 1)" class="p-1 hover:bg-gray-100 text-gray-600"><Plus :size="16"/></button>
                            </div>

                            <button @click="cartStore.removeFromCart(item.productId)" class="font-medium text-red-500 hover:text-red-700 flex items-center gap-1">
                                <Trash2 :size="16" /> Xóa
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Footer -->
        <div v-if="cartStore.items.length > 0" class="border-t border-gray-100 px-6 py-6 bg-gray-50">
            <div class="flex justify-between text-base font-medium text-gray-900 mb-4">
                <p>Tổng cộng</p>
                <p>{{ formatCurrency(cartStore.totalPrice) }}</p>
            </div>
            <p class="mt-0.5 text-sm text-gray-500 mb-6">Phí vận chuyển và thuế sẽ được tính tại trang thanh toán.</p>
            <div class="space-y-3">
                 <button @click="showCheckout = true" class="w-full btn-primary bg-black text-white py-3 rounded-lg font-bold hover:bg-gray-800 transition">Thanh toán ngay</button>
                 <button @click="emit('close')" class="w-full text-center text-sm text-gray-500 hover:text-gray-900">Tiếp tục mua sắm</button>
            </div>
        </div>
      </div>
    </div>
    
    <!-- Nested Checkout Modal -->
    <CheckoutModal v-if="showCheckout" @close="showCheckout = false" @success="handleCheckoutSuccess" />
  </div>
</template>

<style scoped>
@keyframes slide-in-right {
    from { transform: translateX(100%); }
    to { transform: translateX(0); }
}
.animate-slide-in-right {
    animation: slide-in-right 0.3s ease-out;
}
</style>
