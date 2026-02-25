<script setup lang="ts">
import { ref } from 'vue';
import { useCartStore } from '../stores/cart';
import { useAuthStore } from '../stores/auth';
import { createOrder } from '../services/order';
import apiClient from '../utils/api';
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
    note: '',
    paymentMethod: 0 // 0 = COD, 1 = VNPAY, 2 = MOMO
});

const handleSubmit = async () => {
    loading.value = true;
    try {
        const order: any = await createOrder({
            ...formData.value,
            items: cartStore.items.map(i => ({ productId: i.productId, quantity: i.quantity }))
        });

        cartStore.clearCart();
        
        if (formData.value.paymentMethod === 1 || formData.value.paymentMethod === 2) { // VNPAY OR MOMO
            toast.info("Đang chuyển hướng tới cổng thanh toán...");
            const payRes: any = await apiClient.get(`/orders/${order.id}/pay`);
            if (payRes) {
                window.location.href = payRes;
            }
        } else {
            toast.success("Đặt hàng thành công!");
            emit('success');
            emit('close');
        }

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
    <div class="absolute inset-0 bg-black/70 backdrop-blur-sm" @click="$emit('close')"></div>

    <!-- Modal Content -->
    <div class="bg-[#18191c] w-full max-w-lg rounded-2xl shadow-2xl relative overflow-hidden z-10 animate-fade-in-up max-h-[90vh] overflow-y-auto border border-white/10">
        
        <!-- Header -->
        <div class="flex justify-between items-center p-6 border-b border-white/10 sticky top-0 bg-[#18191c] z-20">
            <h3 class="text-xl font-bold text-white font-display">Thông tin giao hàng</h3>
            <button @click="$emit('close')" class="text-gray-400 hover:text-white transition p-2 hover:bg-white/10 rounded-full">
                <X :size="24" />
            </button>
        </div>

        <!-- Body -->
        <div class="p-8">
            <form @submit.prevent="handleSubmit" class="space-y-5">
                
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                     <div>
                        <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Họ tên</label>
                        <input v-model="formData.customerName" type="text" required class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-600" placeholder="Nguyễn Văn A" />
                    </div>
                    <div>
                        <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Số điện thoại</label>
                        <input v-model="formData.customerPhone" type="tel" required class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-600" placeholder="0901234567" />
                    </div>
                </div>

                <div>
                    <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Email</label>
                    <input v-model="formData.customerEmail" type="email" required class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-600" placeholder="email@example.com" />
                </div>

                <div>
                    <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Địa chỉ nhận hàng</label>
                    <textarea v-model="formData.shippingAddress" rows="3" required class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-600" placeholder="Số nhà, đường, phường, quận..."></textarea>
                </div>
                
                 <div>
                    <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Ghi chú (Tùy chọn)</label>
                    <textarea v-model="formData.note" rows="2" class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-600" placeholder="Lời nhắn cho shipper..."></textarea>
                </div>

                <!-- Payment Method Selection -->
                <div>
                     <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Phương thức thanh toán</label>
                     <div class="grid grid-cols-1 md:grid-cols-2 gap-3">
                         <!-- COD -->
                         <div @click="formData.paymentMethod = 0" 
                              :class="['border rounded-xl p-4 cursor-pointer transition flex items-center gap-3', formData.paymentMethod === 0 ? 'border-cyan-400 bg-cyan-400/10 text-cyan-400' : 'border-white/10 text-gray-400 hover:border-white/30 hover:text-white']">
                             <div class="w-4 h-4 rounded-full border-2 flex items-center justify-center transition" :class="formData.paymentMethod === 0 ? 'border-cyan-400' : 'border-gray-500'">
                                 <div v-if="formData.paymentMethod === 0" class="w-2 h-2 rounded-full bg-cyan-400"></div>
                             </div>
                             <span class="font-bold">Thanh toán khi nhận hàng (COD)</span>
                         </div>
                         
                         <!-- VNPAY -->
                         <div @click="formData.paymentMethod = 1" 
                              :class="['border rounded-xl p-4 cursor-pointer transition flex items-center gap-3', formData.paymentMethod === 1 ? 'border-cyan-400 bg-cyan-400/10 text-cyan-400' : 'border-white/10 text-gray-400 hover:border-white/30 hover:text-white']">
                             <div class="w-4 h-4 rounded-full border-2 flex items-center justify-center transition" :class="formData.paymentMethod === 1 ? 'border-cyan-400' : 'border-gray-500'">
                                 <div v-if="formData.paymentMethod === 1" class="w-2 h-2 rounded-full bg-cyan-400"></div>
                             </div>
                             <span class="font-bold">Thanh toán qua VNPay</span>
                         </div>
                     </div>
                </div>

                <div class="bg-white/5 border border-white/10 p-5 rounded-xl flex justify-between items-center text-sm font-medium">
                   <span class="text-gray-300">Tổng thanh toán:</span>
                   <span class="text-xl font-bold text-cyan-400">{{ new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(cartStore.totalPrice) }}</span>
                </div>

                <button type="submit" :disabled="loading" class="w-full btn-primary bg-white text-black hover:bg-cyan-400 transition flex justify-center items-center gap-2 py-4 text-base shadow-none">
                    <span v-if="loading" class="w-4 h-4 border-2 border-black border-t-transparent rounded-full animate-spin"></span>
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
