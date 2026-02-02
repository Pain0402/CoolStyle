<script setup lang="ts">
import { ref } from 'vue';
import { useCartStore } from '../stores/cart';
import { useAuthStore } from '../stores/auth';
import { createOrder } from '../services/order';
import { useToast } from 'vue-toastification';
import { X } from 'lucide-vue-next';

const emit = defineEmits(['close', 'success']);
const cartStore = useCartStore();
const authStore = useAuthStore();
const toast = useToast();

const loading = ref(false);

const formData = ref({
    customerName: authStore.user?.fullName || '',
    customerEmail: authStore.user?.email || '',
    customerPhone: '',
    shippingAddress: '',
    note: ''
});

const handleSubmit = async () => {
    loading.value = true;
    try {
        await createOrder({
            ...formData.value,
            items: cartStore.items.map(i => ({ productId: i.productId, quantity: i.quantity }))
        });

        cartStore.clearCart();
        toast.success("Đặt hàng thành công! Cảm ơn bạn đã mua sắm.");
        emit('success');
        emit('close');
    } catch (err: any) {
        toast.error(err.response?.data?.message || "Có lỗi xảy ra khi đặt hàng.");
    } finally {
        loading.value = false;
    }
};
</script>

<template>
  <div class="fixed inset-0 z-[60] flex items-center justify-center p-4">
    <!-- Backdrop -->
    <div class="absolute inset-0 bg-black/60 backdrop-blur-sm" @click="$emit('close')"></div>

    <!-- Modal Content -->
    <div class="bg-white w-full max-w-lg rounded-2xl shadow-2xl relative overflow-hidden z-10 animate-fade-in-up max-h-[90vh] overflow-y-auto">
        
        <!-- Header -->
        <div class="flex justify-between items-center p-6 border-b sticky top-0 bg-white z-20">
            <h3 class="text-xl font-bold">Thông tin giao hàng</h3>
            <button @click="$emit('close')" class="text-gray-400 hover:text-black transition">
                <X :size="24" />
            </button>
        </div>

        <!-- Body -->
        <div class="p-8">
            <form @submit.prevent="handleSubmit" class="space-y-5">
                
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                     <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Họ tên</label>
                        <input v-model="formData.customerName" type="text" required class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-black outline-none" placeholder="Nguyễn Văn A" />
                    </div>
                    <div>
                        <label class="block text-sm font-medium text-gray-700 mb-1">Số điện thoại</label>
                        <input v-model="formData.customerPhone" type="tel" required class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-black outline-none" placeholder="0901234567" />
                    </div>
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
                    <input v-model="formData.customerEmail" type="email" required class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-black outline-none" placeholder="email@example.com" />
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Địa chỉ nhận hàng</label>
                    <textarea v-model="formData.shippingAddress" rows="3" required class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-black outline-none" placeholder="Số nhà, đường, phường, quận..."></textarea>
                </div>
                
                 <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Ghi chú (Tùy chọn)</label>
                    <textarea v-model="formData.note" rows="2" class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-black outline-none" placeholder="Lời nhắn cho shipper..."></textarea>
                </div>

                <div class="bg-gray-50 p-4 rounded-lg flex justify-between items-center text-sm font-medium">
                   <span>Tổng thanh toán:</span>
                   <span class="text-lg font-bold text-black">{{ new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(cartStore.totalPrice) }}</span>
                </div>

                <button type="submit" :disabled="loading" class="w-full btn-primary flex justify-center items-center gap-2 py-4 text-base">
                    <span v-if="loading" class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
                    {{ loading ? 'Đang xử lý...' : 'Hoàn tất đặt hàng' }}
                </button>
            </form>
        </div>
    </div>
  </div>
</template>

<style scoped>
@keyframes fade-in-up {
    from { opacity: 0; transform: translateY(20px); }
    to { opacity: 1; transform: translateY(0); }
}
.animate-fade-in-up {
    animation: fade-in-up 0.3s ease-out;
}
</style>
