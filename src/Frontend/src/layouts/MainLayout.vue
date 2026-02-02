<script setup lang="ts">
import { ref } from 'vue';
import { ShoppingBag, Search, Menu, User, LogOut } from 'lucide-vue-next';
import { useAuthStore } from '../stores/auth';
import { useCartStore } from '../stores/cart';
import { useToast } from 'vue-toastification';
import AuthModal from '../components/AuthModal.vue';
import CartDrawer from '../components/CartDrawer.vue';

const authStore = useAuthStore();
const cartStore = useCartStore();
const toast = useToast();
const showAuthModal = ref(false);
const showCartDrawer = ref(false);
const showUserMenu = ref(false);

const handleLogout = () => {
  authStore.logout();
  showUserMenu.value = false;
  toast.info("Bạn đã đăng xuất. Hẹn gặp lại! 👋");
};
</script>

<template>
  <div class="min-h-screen flex flex-col font-sans text-gray-900">
    <!-- Navbar -->
    <header class="h-20 border-b border-gray-100 flex items-center justify-between px-6 md:px-12 sticky top-0 bg-white/90 backdrop-blur-md z-50">
      
      <!-- Mobile Menu -->
      <button class="md:hidden p-2">
        <Menu :size="24" />
      </button>

      <!-- Logo -->
      <h1 class="text-2xl font-extrabold tracking-tighter uppercase">Coolstyle.</h1>
      
      <!-- Desktop Nav -->
      <nav class="hidden md:flex gap-8 text-sm font-semibold uppercase tracking-wide">
        <a href="#" class="hover:text-blue-600 transition">Nam</a>
        <a href="#" class="hover:text-blue-600 transition">Nữ</a>
        <a href="#" class="hover:text-blue-600 transition">Bộ sưu tập</a>
        <a href="#" class="text-red-500 hover:text-red-600 transition">Sale</a>
      </nav>
      
      <!-- Icons -->
      <div class="flex gap-6 items-center">
        <button class="hover:text-blue-600 transition"><Search :size="22" stroke-width="2" /></button>
        
        <!-- User Logic -->
        <div class="relative">
          <button v-if="authStore.isAuthenticated" @click="showUserMenu = !showUserMenu" class="hover:text-blue-600 transition flex items-center gap-2">
             <User :size="22" stroke-width="2" />
             <span class="text-sm font-medium hidden lg:block">{{ authStore.user?.fullName }}</span>
          </button>
          <button v-else @click="showAuthModal = true" class="hover:text-blue-600 transition">
             <User :size="22" stroke-width="2" />
          </button>

          <!-- User Dropdown -->
          <div v-if="showUserMenu" class="absolute right-0 mt-4 w-48 bg-white rounded-xl shadow-xl border border-gray-100 py-2 z-50 animate-fade-in-up">
              <a href="#" class="block px-4 py-2 hover:bg-gray-50 text-sm">Tài khoản của tôi</a>
              <a href="#" class="block px-4 py-2 hover:bg-gray-50 text-sm">Đơn hàng</a>
              <div class="border-t my-1"></div>
              <button @click="handleLogout" class="w-full text-left px-4 py-2 hover:bg-gray-50 text-sm text-red-600 flex items-center gap-2">
                 <LogOut :size="16" /> Đăng xuất
              </button>
          </div>
        </div>

        <button @click="showCartDrawer = true" class="hover:text-blue-600 transition relative">
            <ShoppingBag :size="22" stroke-width="2" />
            <span v-if="cartStore.totalItems > 0" class="absolute -top-1 -right-1 bg-red-500 text-white text-[10px] w-4 h-4 flex items-center justify-center rounded-full">{{ cartStore.totalItems }}</span>
        </button>
      </div>
    </header>

    <main class="flex-1">
      <slot />
    </main>

    <AuthModal v-if="showAuthModal" @close="showAuthModal = false" />
    <CartDrawer v-if="showCartDrawer" @close="showCartDrawer = false" />

    <footer class="bg-black text-white py-16 px-6">
      <div class="max-w-7xl mx-auto grid grid-cols-1 md:grid-cols-4 gap-12">
        <div>
           <h4 class="text-lg font-bold mb-4">COOLSTYLE</h4>
           <p class="text-gray-400 text-sm">Thời trang tối giản, chất lượng cao dành cho người Việt.</p>
        </div>
        <div>
           <h4 class="font-bold mb-4">Về chúng tôi</h4>
           <ul class="space-y-2 text-sm text-gray-400">
             <li>Câu chuyện</li>
             <li>Tuyển dụng</li>
           </ul>
        </div>
         <div>
           <h4 class="font-bold mb-4">Hỗ trợ</h4>
           <ul class="space-y-2 text-sm text-gray-400">
             <li>Chính sách đổi trả</li>
             <li>Bảo mật</li>
           </ul>
        </div>
      </div>
      <div class="mt-12 text-center text-xs text-gray-600 border-t border-gray-800 pt-8">
        © 2026 Coolstyle. All rights reserved.
      </div>
    </footer>
  </div>
</template>

<style scoped>
@keyframes fade-in-up {
    from { opacity: 0; transform: translateY(10px); }
    to { opacity: 1; transform: translateY(0); }
}
.animate-fade-in-up {
    animation: fade-in-up 0.2s ease-out;
}
</style>
