<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import { useToast } from 'vue-toastification';
import { ArrowLeft } from 'lucide-vue-next';

const email = ref('');
const password = ref('');
const authStore = useAuthStore();
const router = useRouter();
const toast = useToast();
const loading = ref(false);

const handleLogin = async () => {
    loading.value = true;
    try {
        const success = await authStore.login({ email: email.value, password: password.value });
        if (success) {
            toast.success("Chào mừng trở lại! 🚀");
            router.push('/');
        } else {
            toast.error("Sai email hoặc mật khẩu");
        }
    } catch (e) {
        toast.error("Lỗi đăng nhập");
    } finally {
        loading.value = false;
    }
};
</script>

<template>
  <div class="h-screen w-full grid grid-cols-1 md:grid-cols-2 overflow-hidden bg-[#050505]">
    <!-- Left: Image -->
    <div class="hidden md:block relative h-full">
         <img src="https://images.unsplash.com/photo-1515886657613-9f3515b0c78f?ixlib=rb-1.2.1&auto=format&fit=crop&w=1000&q=80" 
              class="w-full h-full object-cover grayscale opacity-60" alt="Login Banner">
         <div class="absolute inset-0 bg-gradient-to-r from-transparent to-[#050505]"></div>
         <div class="absolute bottom-20 left-12 text-white p-6 glass rounded-2xl max-w-md">
             <h2 class="text-3xl font-display font-bold mb-4">Welcome Back</h2>
             <p class="text-gray-300">Khám phá bộ sưu tập mới nhất và nhận ưu đãi độc quyền.</p>
         </div>
    </div>

    <!-- Right: Form -->
    <div class="relative flex flex-col justify-center px-10 sm:px-20 lg:px-32 bg-[#050505] text-white">
         <router-link to="/" class="absolute top-10 left-10 flex items-center gap-2 text-gray-500 hover:text-white transition">
             <ArrowLeft :size="20"/> Home
         </router-link>

         <div class="mb-10">
             <h1 class="font-display text-5xl font-black mb-2 text-transparent bg-clip-text bg-gradient-to-r from-white to-gray-500">Log In.</h1>
             <p class="text-gray-400">Sign in to your CoolStyle account</p>
         </div>

         <form @submit.prevent="handleLogin" class="space-y-6">
             <div>
                 <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Email</label>
                 <input v-model="email" type="email" required 
                        class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-700"
                        placeholder="you@example.com">
             </div>
             
             <div>
                 <div class="flex justify-between items-center mb-2">
                    <label class="block text-xs font-bold uppercase text-gray-500">Password</label>
                    <a href="#" class="text-xs text-cyan-500 hover:text-cyan-400">Forgot?</a>
                 </div>
                 <input v-model="password" type="password" required 
                        class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-700"
                        placeholder="••••••••">
             </div>

             <button :disabled="loading" type="submit" 
                     class="w-full btn-primary bg-white text-black hover:bg-cyan-400 transition flex items-center justify-center gap-2">
                 <span v-if="loading" class="animate-spin w-5 h-5 border-2 border-black border-t-transparent rounded-full"></span>
                 {{ loading ? 'Signing in...' : 'Sign In' }}
             </button>
         </form>

         <p class="mt-8 text-center text-gray-400 text-sm">
             Don't have an account? <router-link to="/register" class="text-cyan-500 font-bold hover:underline">Sign up</router-link>
         </p>
    </div>
  </div>
</template>
