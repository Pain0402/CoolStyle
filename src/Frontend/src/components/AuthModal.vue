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
    <div class="bg-white w-full max-w-md rounded-2xl shadow-2xl relative overflow-hidden z-10 animate-fade-in-up">
        
        <!-- Header -->
        <div class="flex justify-between items-center p-6 border-b">
            <h3 class="text-xl font-bold">{{ isLogin ? 'Đăng nhập' : 'Đăng ký tài khoản' }}</h3>
            <button @click="$emit('close')" class="text-gray-400 hover:text-black transition">
                <X :size="24" />
            </button>
        </div>

        <!-- Body -->
        <div class="p-8">
            <form @submit.prevent="handleSubmit" class="space-y-5">
                
                <div v-if="!isLogin">
                    <label class="block text-sm font-medium text-gray-700 mb-1">Họ tên</label>
                    <input v-model="formData.fullName" type="text" required class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-black focus:border-black outline-none transition" placeholder="Nguyễn Văn A" />
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Email</label>
                    <input v-model="formData.email" type="email" required class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-black focus:border-black outline-none transition" placeholder="email@example.com" />
                </div>

                <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1">Mật khẩu</label>
                    <input v-model="formData.password" type="password" required class="w-full px-4 py-3 border border-gray-300 rounded-lg focus:ring-2 focus:ring-black focus:border-black outline-none transition" placeholder="••••••••" />
                </div>

                <button type="submit" :disabled="loading" class="w-full btn-primary flex justify-center items-center gap-2">
                    <span v-if="loading" class="w-4 h-4 border-2 border-white border-t-transparent rounded-full animate-spin"></span>
                    {{ isLogin ? 'Đăng nhập' : 'Tạo tài khoản' }}
                </button>
            </form>

            <div class="mt-6 text-center text-sm text-gray-500">
                <span v-if="isLogin">Chưa có tài khoản? <button @click="isLogin = false" class="text-black font-semibold hover:underline">Đăng ký ngay</button></span>
                <span v-else>Đã có tài khoản? <button @click="isLogin = true" class="text-black font-semibold hover:underline">Đăng nhập</button></span>
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
