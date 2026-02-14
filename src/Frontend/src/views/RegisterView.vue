<script setup lang="ts">
import { ref } from 'vue';
import { useRouter } from 'vue-router';
import { useAuthStore } from '../stores/auth';
import { useToast } from 'vue-toastification';
import { ArrowLeft } from 'lucide-vue-next';

const fullName = ref('');
const email = ref('');
const password = ref('');
const authStore = useAuthStore();
const router = useRouter();
const toast = useToast();
const loading = ref(false);

const handleRegister = async () => {
    loading.value = true;
    try {
        const success = await authStore.register({ fullName: fullName.value, email: email.value, password: password.value });
        if (success) {
            toast.success("Đăng ký thành công! 🎉");
            router.push('/');
        }
    } catch (e: any) {
        toast.error(e.response?.data?.message || "Đăng ký thất bại");
    } finally {
        loading.value = false;
    }
};
</script>

<template>
  <div class="min-h-screen w-full grid grid-cols-1 md:grid-cols-2 bg-[#050505]">
    <!-- Left: Image -->
    <div class="hidden md:block relative h-full">
         <img src="https://images.unsplash.com/photo-1483985988355-763728e1935b?ixlib=rb-1.2.1&auto=format&fit=crop&w=1000&q=80" 
              class="w-full h-full object-cover grayscale opacity-60" alt="Register Banner">
         <div class="absolute inset-0 bg-gradient-to-r from-transparent to-[#050505]"></div>
         <div class="absolute bottom-20 left-12 text-white p-6 glass rounded-2xl max-w-md">
             <h2 class="text-3xl font-display font-bold mb-4">Join the Movement</h2>
             <p class="text-gray-300">Trở thành thành viên CoolStyle để nhận ưu đãi sớm nhất.</p>
         </div>
    </div>

    <!-- Right: Form -->
    <div class="relative flex flex-col justify-center px-10 sm:px-20 lg:px-32 bg-[#050505] text-white py-12">
         <router-link to="/" class="absolute top-10 left-10 flex items-center gap-2 text-gray-500 hover:text-white transition">
             <ArrowLeft :size="20"/> Home
         </router-link>

         <div class="mb-10">
             <h1 class="font-display text-5xl font-black mb-2 text-transparent bg-clip-text bg-gradient-to-r from-white to-gray-500">Create Account.</h1>
             <p class="text-gray-400">Join CoolStyle today</p>
         </div>

         <form @submit.prevent="handleRegister" class="space-y-6">
             <div>
                 <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Full Name</label>
                 <input v-model="fullName" type="text" required 
                        class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-700"
                        placeholder="John Doe">
             </div>

             <div>
                 <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Email</label>
                 <input v-model="email" type="email" required 
                        class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-700"
                        placeholder="you@example.com">
             </div>
             
             <div>
                 <label class="block text-xs font-bold uppercase text-gray-500 mb-2">Password</label>
                 <input v-model="password" type="password" required 
                        class="w-full bg-white/5 border border-white/10 rounded-xl px-4 py-3 text-white focus:outline-none focus:border-cyan-500 focus:ring-1 focus:ring-cyan-500 transition placeholder-gray-700"
                        placeholder="••••••••">
             </div>

             <button :disabled="loading" type="submit" 
                     class="w-full btn-primary bg-white text-black hover:bg-cyan-400 transition flex items-center justify-center gap-2">
                 <span v-if="loading" class="animate-spin w-5 h-5 border-2 border-black border-t-transparent rounded-full"></span>
                 {{ loading ? 'Creating Account...' : 'Sign Up' }}
             </button>
         </form>

         <p class="mt-8 text-center text-gray-400 text-sm">
             Already have an account? <router-link to="/login" class="text-cyan-500 font-bold hover:underline">Log In</router-link>
         </p>
    </div>
  </div>
</template>
