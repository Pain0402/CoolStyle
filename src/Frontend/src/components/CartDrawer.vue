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
    <div class="absolute inset-0 bg-black/60 backdrop-blur-sm transition-opacity opacity-100" @click="emit('close')"></div>

    <!-- Drawer -->
    <div class="absolute inset-y-0 right-0 max-w-md w-full flex">
      <div class="h-full flex flex-col bg-[#18191c] shadow-2xl animate-slide-in-right w-full border-l border-white/10">
        
        <!-- Header -->
        <div class="flex items-center justify-between px-6 py-5 border-b border-white/10">
          <div class="flex items-center gap-3 text-white">
             <ShoppingBag :size="20" />
             <h2 class="text-lg font-bold font-display">Giỏ hàng ({{ cartStore.totalItems }})</h2>
          </div>
          <button @click="emit('close')" class="text-gray-400 hover:text-white transition p-2 hover:bg-white/10 rounded-full">
            <X :size="20" />
          </button>
        </div>

        <!-- Scrollable Content -->
        <div class="flex-1 overflow-y-auto p-6 space-y-6">
            <div v-if="cartStore.items.length === 0" class="text-center py-20 text-gray-400">
                <ShoppingBag :size="48" class="mx-auto mb-4 opacity-20" />
                <p>Giỏ hàng của bạn đang trống.</p>
                <button @click="emit('close')" class="mt-4 text-cyan-400 font-bold hover:underline">Tiếp tục mua sắm</button>
            </div>

            <div v-else class="space-y-6">
                <div v-for="item in cartStore.items" :key="item.productId" class="flex gap-4">
                    <!-- Image -->
                    <div class="h-24 w-20 flex-shrink-0 overflow-hidden rounded-xl border border-white/10 bg-white/5">
                        <img :src="item.thumbnailUrl || 'https://placehold.co/100'" :alt="item.name" class="h-full w-full object-cover object-center" />
                    </div>

                    <!-- Details -->
                    <div class="flex flex-1 flex-col">
                        <div>
                            <div class="flex justify-between text-base font-medium text-white">
                                <h3 class="line-clamp-2 hover:text-cyan-400 transition"><a :href="`/product/${item.slug}`">{{ item.name }}</a></h3>
                                <p class="ml-4 text-cyan-400">{{ formatCurrency(item.price * item.quantity) }}</p>
                            </div>
                            <p class="mt-1 text-sm text-gray-400">Size: {{ item.size }} | Màu: {{ item.color }}</p>
                        </div>
                        <div class="flex flex-1 items-end justify-between text-sm">
                            <div class="flex items-center border border-white/10 rounded-lg bg-white/5">
                                <button @click="cartStore.updateQuantity(item.productId, item.quantity - 1)" class="p-1.5 hover:bg-white/10 text-gray-400 hover:text-white transition"><Minus :size="14"/></button>
                                <span class="px-3 font-medium text-white">{{ item.quantity }}</span>
                                <button @click="cartStore.updateQuantity(item.productId, item.quantity + 1)" class="p-1.5 hover:bg-white/10 text-gray-400 hover:text-white transition"><Plus :size="14"/></button>
                            </div>

                            <button @click="cartStore.removeFromCart(item.productId)" class="font-medium text-red-400 hover:text-red-300 flex items-center gap-1 transition">
                                <Trash2 :size="16" />
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>

        <!-- Footer -->
        <div v-if="cartStore.items.length > 0" class="border-t border-white/10 px-6 py-6 bg-[#0f1012]">
            <div class="flex justify-between text-base font-medium text-white mb-4">
                <p>Tổng cộng</p>
                <p class="text-xl font-bold">{{ formatCurrency(cartStore.totalPrice) }}</p>
            </div>
            <p class="mt-0.5 text-sm text-gray-500 mb-6">Phí vận chuyển và thuế sẽ được tính tại trang thanh toán.</p>
            <div class="space-y-3">
                 <button @click="showCheckout = true" class="w-full btn-primary bg-white text-black hover:bg-cyan-400 transition shadow-none hover:shadow-[0_0_15px_rgba(0,242,234,0.4)]">Thanh toán ngay</button>
                 <button @click="emit('close')" class="w-full text-center text-sm text-gray-400 hover:text-white transition">Tiếp tục mua sắm</button>
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
