<script setup lang="ts">
import { ref, onMounted, watch } from 'vue';
import { ShoppingBag, Search, Menu, User, LogOut, Instagram, Facebook, Twitter, ArrowRight, Heart, X, Loader2 } from 'lucide-vue-next';
import { useAuthStore } from '../stores/auth';
import { useCartStore } from '../stores/cart';
import { useWishlistStore } from '../stores/wishlist';
import { useToast } from 'vue-toastification';
import apiClient from '../utils/api';
import AuthModal from '../components/AuthModal.vue';
import CartDrawer from '../components/CartDrawer.vue';
import { useRouter } from 'vue-router';

const authStore = useAuthStore();
const cartStore = useCartStore();
const wishlistStore = useWishlistStore();
const toast = useToast();
const router = useRouter();

const showAuthModal = ref(false);
const showCartDrawer = ref(false);
const showUserMenu = ref(false);
const categories = ref<any[]>([]);

// Search State
const showSearch = ref(false);
const searchQuery = ref('');
const searchResults = ref<any[]>([]);
const isSearching = ref(false);
let searchTimeout: any = null;

const fetchSuggestions = async () => {
    if (searchQuery.value.length < 2) {
        searchResults.value = [];
        return;
    }

    isSearching.value = true;
    try {
        const res: any = await apiClient.get(`/products/search?q=${searchQuery.value}`);
        searchResults.value = res || [];
    } catch (e) {
        console.error("Search failed", e);
    } finally {
        isSearching.value = false;
    }
};

watch(searchQuery, () => {
    clearTimeout(searchTimeout);
    searchTimeout = setTimeout(fetchSuggestions, 300);
});

const goToProduct = (slug: string) => {
    router.push(`/product/${slug}`);
    showSearch.value = false;
    searchQuery.value = '';
    searchResults.value = [];
};

const handleSearchEnter = () => {
    if (searchQuery.value.trim()) {
        router.push(`/shop?search=${encodeURIComponent(searchQuery.value.trim())}`);
        showSearch.value = false;
        searchQuery.value = '';
        searchResults.value = [];
    }
};

const toggleSearch = () => {
    showSearch.value = !showSearch.value;
    if (!showSearch.value) {
        searchQuery.value = '';
        searchResults.value = [];
    }
};

const formatCurrency = (val: number) => new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(val);

// Custom Directives
const vFocus = {
  mounted: (el: any) => el.focus()
}

const fetchCategories = async () => {
    try {
        const res: any = await apiClient.get('/categories');
        if (res) categories.value = res;
    } catch (e) {
        console.error("Failed to fetch menu categories", e);
    }
}

onMounted(() => {
    fetchCategories();
    if (authStore.isAuthenticated) {
        wishlistStore.fetchWishlist();
    }
});

const handleLogout = () => {
  authStore.logout();
  wishlistStore.items = []; // Clear wishlist on logout
  showUserMenu.value = false;
  toast.info("Bạn đã đăng xuất. Hẹn gặp lại! 👋");
};
</script>

<template>
  <div class="min-h-screen flex flex-col font-sans text-white selection:bg-cyan-500/30 selection:text-cyan-100">
    <!-- Navbar (Neo-Glass) -->
    <header class="h-20 fixed w-full top-0 z-50 transition-all duration-300 glass">
      <div class="h-full container mx-auto px-6 flex items-center justify-between">
          
          <!-- Mobile Menu -->
          <button class="md:hidden p-2 text-white hover:text-cyan-400 transition">
            <Menu :size="24" />
          </button>

          <!-- Logo -->
          <router-link to="/" class="text-2xl font-display font-black tracking-[0.1em] uppercase bg-clip-text text-transparent bg-gradient-to-r from-white to-gray-400 hover:to-cyan-400 transition-colors">
            CoolStyle
          </router-link>
          
          <!-- Desktop Nav -->
          <nav class="hidden md:flex gap-10 text-sm font-bold uppercase tracking-widest">
            <router-link to="/" class="nav-link">Home</router-link>
            
            <!-- Dynamic Categories -->
            <router-link v-for="cat in categories" :key="cat.id" :to="`/shop?category=${cat.slug}`" class="nav-link">
                {{ cat.name }}
            </router-link>

            <!-- Fallback if no categories -->
            <template v-if="categories.length === 0">
                <router-link to="/shop?gender=men" class="nav-link">Men</router-link>
                <router-link to="/shop?gender=women" class="nav-link">Women</router-link>
            </template>

            <router-link to="/shop" class="nav-link text-neon">Collections</router-link>
            <router-link to="/shop" class="text-red-500 hover:text-red-400 hover:drop-shadow-[0_0_8px_rgba(239,68,68,0.5)] transition">Sale</router-link>
          </nav>
          
          <!-- Icons -->
          <div class="flex gap-6 items-center">
            
            <!-- Inline Expanding Search Bar -->
            <div class="relative flex items-center">
                <div :class="['flex items-center bg-white/5 border border-white/10 rounded-full transition-all duration-500 ease-in-out px-3 overflow-hidden', 
                              showSearch ? 'w-[250px] h-10 border-cyan-500/50 bg-white/10 shadow-[0_0_15px_rgba(34,211,238,0.2)]' : 'w-10 h-10 border-transparent cursor-pointer hover:bg-white/10']"
                     @click="!showSearch && toggleSearch()">
                    <Search :size="18" :class="['transition-colors duration-300 flex-shrink-0', showSearch ? 'text-cyan-400' : 'text-gray-400']" />
                    <input 
                        v-if="showSearch"
                        v-model="searchQuery"
                        v-focus
                        type="text" 
                        placeholder="Search..."
                        class="bg-transparent border-none outline-none text-sm ml-3 w-full text-white placeholder-gray-500"
                        @keyup.enter="handleSearchEnter"
                        @blur="!searchQuery && (showSearch = false)"
                        @keyup.esc="showSearch = false"
                    >
                    <Loader2 v-if="isSearching && showSearch" class="ml-2 text-cyan-400 animate-spin flex-shrink-0" :size="14" />
                    <button v-if="showSearch && searchQuery" @click.stop="searchQuery = ''" class="ml-2 text-gray-500 hover:text-white flex-shrink-0">
                        <X :size="14" />
                    </button>
                </div>

                <!-- Inline Suggestions Dropdown -->
                <transition name="fade">
                    <div v-if="showSearch && searchResults.length > 0" 
                         class="absolute top-12 left-0 right-0 glass border border-white/10 rounded-2xl py-3 z-[60] shadow-2xl max-h-[400px] overflow-y-auto custom-scrollbar animate-fade-in-up">
                        <div v-for="product in searchResults" :key="product.id"
                             @click="goToProduct(product.slug)"
                             class="px-4 py-3 hover:bg-white/5 flex items-center gap-4 cursor-pointer group transition">
                            <img :src="product.thumbnailUrl" class="w-10 h-12 object-cover rounded-md border border-white/10" />
                            <div class="flex-1 min-w-0">
                                <h5 class="text-sm font-bold truncate group-hover:text-cyan-400 transition">{{ product.name }}</h5>
                                <p class="text-[10px] text-cyan-500/70 uppercase tracking-widest">{{ product.categoryName }}</p>
                            </div>
                            <div class="text-xs font-black">{{ formatCurrency(product.basePrice) }}</div>
                        </div>
                    </div>
                </transition>
            </div>
            
            <!-- Wishlist Link -->
            <router-link to="/profile" @click="() => {}" class="icon-btn relative group focus:outline-none">
                <Heart :size="20" stroke-width="2.5" />
                <span v-if="wishlistStore.items.length > 0" class="absolute -top-1 -right-1 bg-red-500 text-white text-[10px] w-4 h-4 flex items-center justify-center rounded-full font-bold shadow-[0_0_8px_rgba(239,68,68,0.5)]">
                    {{ wishlistStore.items.length }}
                </span>
            </router-link>

            <!-- User Logic -->
            <div class="relative">
              <button v-if="authStore.isAuthenticated" @click="showUserMenu = !showUserMenu" class="icon-btn flex items-center gap-3 p-1.5 pr-4 group">
                 <div class="w-9 h-9 rounded-full overflow-hidden border border-white/10 group-hover:border-cyan-500 transition-all shadow-[0_0_10px_rgba(255,255,255,0.05)]">
                    <img v-if="authStore.user?.avatarUrl" :src="authStore.user.avatarUrl" alt="Avatar" class="w-full h-full object-cover" />
                    <div v-else class="w-full h-full bg-gradient-to-br from-cyan-400 to-blue-600 flex items-center justify-center text-xs font-bold text-black">
                        {{ authStore.user?.fullName?.charAt(0) }}
                    </div>
                 </div>
                 <span class="text-xs font-bold hidden lg:block text-gray-400 group-hover:text-white transition-colors">{{ authStore.user?.fullName }}</span>
              </button>
              <button v-else @click="showAuthModal = true" class="icon-btn">
                 <User :size="20" stroke-width="2.5" />
              </button>

              <!-- User Dropdown (Glass) -->
              <div v-if="showUserMenu" class="absolute right-0 mt-6 w-56 glass rounded-xl py-2 z-50 animate-fade-in-up border border-white/10">
                  <div class="px-4 py-3 border-b border-white/10 mb-2">
                      <p class="text-xs text-gray-400">Signed in as</p>
                      <p class="font-bold text-white truncate">{{ authStore.user?.email }}</p>
                  </div>
                  <router-link to="/profile" class="block px-4 py-2 hover:bg-white/10 text-sm text-gray-300 hover:text-white transition">My Profile</router-link>
                  <router-link to="/profile" class="block px-4 py-2 hover:bg-white/10 text-sm text-gray-300 hover:text-white transition">Orders</router-link>
                  <div class="border-t border-white/10 my-1"></div>
                  <button @click="handleLogout" class="w-full text-left px-4 py-2 hover:bg-red-500/20 text-sm text-red-500 hover:text-red-400 flex items-center gap-2 transition">
                     <LogOut :size="16" /> Sign out
                  </button>
              </div>
            </div>

            <button @click="showCartDrawer = true" class="icon-btn relative group">
                <ShoppingBag :size="20" stroke-width="2.5" />
                <span v-if="cartStore.totalItems > 0" class="absolute -top-1 -right-1 bg-[#00f2ea] text-black text-[10px] w-4 h-4 flex items-center justify-center rounded-full font-bold shadow-[0_0_10px_rgba(0,242,234,0.6)] animate-pulse">
                    {{ cartStore.totalItems }}
                </span>
            </button>
          </div>
      </div>
    </header>

    <main class="flex-1 pt-20">
      <slot />
    </main>

    <AuthModal v-if="showAuthModal" @close="showAuthModal = false" />
    <CartDrawer v-if="showCartDrawer" @close="showCartDrawer = false" />

    <!-- Huge Footer -->
    <footer class="bg-black text-white pt-24 pb-12 border-t border-white/5 relative overflow-hidden">
       <!-- Decoration -->
       <h1 class="absolute bottom-0 left-1/2 -translate-x-1/2 text-[15vw] font-black text-white/5 whitespace-nowrap select-none pointer-events-none font-display">
           COOLSTYLE
       </h1>

      <div class="container mx-auto px-6 grid grid-cols-1 md:grid-cols-4 gap-12 relative z-10 mb-20">
        <div>
           <h4 class="text-2xl font-display font-bold mb-6">COOLSTYLE.</h4>
           <p class="text-gray-400 text-sm leading-relaxed mb-6">
               Định hình phong cách tương lai. <br>
               Chất lượng vượt trội, thiết kế đột phá.
           </p>
           <div class="flex gap-4">
               <a href="#" class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center hover:bg-[#00f2ea] hover:text-black hover:border-transparent transition-all duration-300">
                   <Instagram :size="18" />
               </a>
               <a href="#" class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center hover:bg-blue-600 hover:text-white hover:border-transparent transition-all duration-300">
                   <Facebook :size="18" />
               </a>
               <a href="#" class="w-10 h-10 rounded-full bg-white/5 border border-white/10 flex items-center justify-center hover:bg-sky-400 hover:text-white hover:border-transparent transition-all duration-300">
                   <Twitter :size="18" />
               </a>
           </div>
        </div>
        
        <!-- Links -->
        <div>
           <h4 class="font-bold mb-6 uppercase tracking-widest text-sm">Shop</h4>
           <ul class="space-y-3 text-sm text-gray-400">
             <li><a href="#" class="hover:text-cyan-400 transition">All Products</a></li>
             <li><a href="#" class="hover:text-cyan-400 transition">New Arrivals</a></li>
             <li><a href="#" class="hover:text-cyan-400 transition">Accessories</a></li>
             <li><a href="#" class="hover:text-cyan-400 transition">Sales</a></li>
           </ul>
        </div>
         <div>
           <h4 class="font-bold mb-6 uppercase tracking-widest text-sm">Support</h4>
           <ul class="space-y-3 text-sm text-gray-400">
             <li><a href="#" class="hover:text-cyan-400 transition">FAQ</a></li>
             <li><a href="#" class="hover:text-cyan-400 transition">Return Policy</a></li>
             <li><a href="#" class="hover:text-cyan-400 transition">Terms of Service</a></li>
             <li><a href="#" class="hover:text-cyan-400 transition">Privacy</a></li>
           </ul>
        </div>
        
        <!-- Newsletter -->
        <div>
            <h4 class="font-bold mb-6 uppercase tracking-widest text-sm">Stay in the loop</h4>
            <div class="flex">
                <input type="email" placeholder="Enter your email" class="bg-white/5 border border-white/10 rounded-l-lg px-4 py-3 text-sm w-full focus:outline-none focus:border-cyan-500/50 transition placeholder-gray-600">
                <button class="bg-white text-black font-bold px-4 rounded-r-lg hover:bg-cyan-400 transition">
                    <ArrowRight :size="18" />
                </button>
            </div>
        </div>
      </div>
      
      <div class="container mx-auto px-6 text-center text-xs text-gray-700 font-medium">
        © 2026 Coolstyle Inc. Designed for the Future.
      </div>
    </footer>
  </div>
</template>

<style scoped>
.nav-link {
    color: #cbd5e1; /* gray-300 */
    position: relative;
    transition: color 150ms;
}
.nav-link:hover {
    color: #ffffff;
}

.nav-link::after {
    content: '';
    position: absolute;
    bottom: -4px;
    left: 0;
    width: 0;
    height: 2px;
    background-color: #22d3ee; /* cyan-400 */
    transition: all 300ms;
}
.nav-link:hover::after {
    width: 100%;
}

.icon-btn {
    color: #9ca3af; /* gray-400 */
    padding: 0.5rem; /* p-2 */
    border-radius: 9999px; /* rounded-full */
    transition: all 300ms;
    backdrop-filter: blur(4px); /* backdrop-blur-sm */
}
.icon-btn:hover {
    color: #ffffff;
    background-color: #1f2937; /* bg-gray-800 */
}
.icon-btn:active {
    transform: scale(0.95);
}

@keyframes fade-in-up {
    from { opacity: 0; transform: translateY(30px); }
    to { opacity: 1; transform: translateY(0); }
}
.animate-fade-in-up {
    animation: fade-in-up 0.8s cubic-bezier(0.16, 1, 0.3, 1) both;
}

@keyframes staggered-in {
    from { opacity: 0; transform: scale(0.9) translateY(20px); }
    to { opacity: 1; transform: scale(1) translateY(0); }
}
.animate-staggered-in {
    animation: staggered-in 0.6s cubic-bezier(0.34, 1.56, 0.64, 1) both;
}

@keyframes fade-in {
    from { opacity: 0; }
    to { opacity: 1; }
}
.animate-fade-in {
    animation: fade-in 0.4s ease-out both;
}

/* Vue Transitions */
.search-overlay-enter-active, .search-overlay-leave-active {
    transition: all 0.5s cubic-bezier(0.16, 1, 0.3, 1);
}
.search-overlay-enter-from, .search-overlay-leave-to {
    opacity: 0;
    backdrop-filter: blur(0px);
}

.fade-enter-active, .fade-leave-active {
    transition: opacity 0.3s;
}
.fade-enter-from, .fade-leave-to {
    opacity: 0;
}

.custom-scrollbar::-webkit-scrollbar {
    width: 6px;
}
.custom-scrollbar::-webkit-scrollbar-track {
    background: transparent;
}
.custom-scrollbar::-webkit-scrollbar-thumb {
    background: rgba(255, 255, 255, 0.05);
    border-radius: 10px;
}
.custom-scrollbar::-webkit-scrollbar-thumb:hover {
    background: rgba(34, 211, 238, 0.2);
}

/* Glass Utility */
.glass {
    background: rgba(0, 0, 0, 0.7);
    backdrop-filter: blur(20px);
    border-bottom: 1px solid rgba(255, 255, 255, 0.05);
}
</style>
