<script setup lang="ts">
import { ref } from 'vue';
import { useAuthStore } from '../stores/auth';
import { useToast } from 'vue-toastification';
import { X } from 'lucide-vue-next';

const emit = defineEmits(['close']);
const authStore = useAuthStore();
const toast = useToast();

const isLogin = ref(true);
const loading = ref(false);

const formData = ref({
    email: '',
    password: '',
    fullName: ''
});

const handleSubmit = async () => {
    loading.value = true;
    
    try {
        if (isLogin.value) {
            await authStore.login({ email: formData.value.email, password: formData.value.password });
            toast.success("Chào mừng bạn quay trở lại! 🎉");
        } else {
            await authStore.register(formData.value);
            toast.success("Đăng ký tài khoản thành công! 🚀");
        }
        emit('close');
    } catch (err: any) {
        const errorMsg = err.response?.data?.message || err.message || 'Có lỗi xảy ra.';
        toast.error(errorMsg);
    } finally {
        loading.value = false;
    }
};
</script>

<template>
  <div class="fixed inset-0 z-50 flex items-center justify-center p-4">
    <!-- Backdrop -->
    <div class="absolute inset-0 bg-black/60 backdrop-blur-sm" @click="$emit('close')"></div>

    <!-- Modal Content -->
    <div class="bg-[#18191c] w-full max-w-md rounded-2xl shadow-2xl relative overflow-hidden z-10 animate-fade-in-up border border-white/10">
        
        <!-- Header -->
        <div class="flex justify-between items-center p-6 border-b border-white/10">
            <h3 class="text-xl font-bold text-white font-display">{{ isLogin ? 'Đăng nhập' : 'Đăng ký tài khoản' }}</h3>
            <button @click="$emit('close')" class="text-gray-400 hover:text-white transition p-2 hover:bg-white/10 rounded-full">
                <X :size="20" />
            </button>
        </div>

        <!-- Body -->
        <div class="p-8">
            <form @submit.prevent="handleSubmit" class="space-y-5">
                
                <div v-if="!isLogin">
                    <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Họ tên</label>
                    <input v-model="formData.fullName" type="text" required class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-600" placeholder="Nguyễn Văn A" />
                </div>

                <div>
                    <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Email</label>
                    <input v-model="formData.email" type="email" required class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-600" placeholder="email@example.com" />
                </div>

                <div>
                    <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Mật khẩu</label>
                    <input v-model="formData.password" type="password" required class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-600" placeholder="••••••••" />
                </div>

                <button type="submit" :disabled="loading" class="w-full btn-primary bg-white text-black hover:bg-cyan-400 transition flex justify-center items-center gap-2 shadow-none hover:shadow-[0_0_15px_rgba(0,242,234,0.4)]">
                    <span v-if="loading" class="w-4 h-4 border-2 border-black border-t-transparent rounded-full animate-spin"></span>
                    {{ isLogin ? 'Đăng nhập' : 'Tạo tài khoản' }}
                </button>
            </form>

            <div class="mt-6 text-center text-sm text-gray-400">
                <span v-if="isLogin">Chưa có tài khoản? <button @click="isLogin = false" class="text-white font-bold hover:text-cyan-400">Đăng ký ngay</button></span>
                <span v-else>Đã có tài khoản? <button @click="isLogin = true" class="text-white font-bold hover:text-cyan-400">Đăng nhập</button></span>
            </div>
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
