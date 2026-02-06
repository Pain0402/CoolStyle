<script setup lang="ts">
import { useCartStore } from '../stores/cart';
import { useToast } from 'vue-toastification';
import { ShoppingBag, Heart, Eye } from 'lucide-vue-next';

interface Product {
  id: number;
  name: string;
  slug: string;
  basePrice: number;
  thumbnailUrl: string;
  categoryName: string;
  isNew?: boolean; // Optional property for UI
  discount?: number; // Optional property for UI
}

const props = defineProps<{ product: Product }>();
const cartStore = useCartStore();
const toast = useToast();

const addToCart = (e: Event) => {
    e.stopPropagation(); // Prevent navigating to detail page if we click add to cart
    cartStore.addToCart(props.product);
    toast.success(`Đã thêm "${props.product.name}" vào giỏ hàng!`);
};

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

// Mock "New" badge for recent items (purely visible logic for now)
const isNew = props.product.id > 300 || props.product.isNew;
</script>

<template>
  <router-link :to="`/product/${product.slug}`" class="group relative block h-full">
    <!-- Glow Effect Background -->
    <div class="absolute -inset-0.5 bg-gradient-to-r from-cyan-500 to-purple-600 rounded-2xl opacity-0 group-hover:opacity-75 blur transition duration-500 will-change-[opacity]"></div>
    
    <!-- Main Card Content -->
    <div class="relative h-full bg-[#0f1012] rounded-2xl overflow-hidden border border-white/5 flex flex-col transition-transform duration-300 group-hover:-translate-y-1">
      
      <!-- Image Section -->
      <div class="relative aspect-[3/4] overflow-hidden bg-white/5">
        <!-- New Badge -->
        <span v-if="isNew" class="absolute top-3 left-3 z-20 px-3 py-1 text-xs font-bold text-black bg-[#00f2ea] rounded-full shadow-[0_0_10px_rgba(0,242,234,0.5)]">
          NEW
        </span>

        <!-- Wishlist Button -->
        <button class="absolute top-3 right-3 z-20 p-2 text-white/70 hover:text-red-500 hover:bg-white/10 rounded-full backdrop-blur-md transition-colors duration-300">
           <Heart :size="20" />
        </button>

        <!-- Product Image -->
        <img 
          :src="product.thumbnailUrl || 'https://placehold.co/400x600/1a1a1a/FFF?text=CoolStyle'" 
          alt="Product" 
          loading="lazy"
          class="w-full h-full object-cover transition-transform duration-700 ease-out group-hover:scale-110"
        />

        <!-- Overlay Actions -->
        <div class="absolute inset-0 bg-black/40 backdrop-blur-[2px] opacity-0 group-hover:opacity-100 transition-opacity duration-300 flex flex-col justify-end p-4">
             <button 
                @click="addToCart"
                class="w-full flex items-center justify-center gap-2 bg-white text-black font-bold py-3 rounded-xl hover:bg-[#00f2ea] hover:shadow-[0_0_15px_rgba(0,242,234,0.4)] transition-all duration-300 transform translate-y-4 group-hover:translate-y-0"
             >
                <ShoppingBag :size="18" /> Thêm vào giỏ
             </button>
        </div>
      </div>

      <!-- Content Section -->
      <div class="p-4 flex flex-col flex-1 gap-2">
         <!-- Category -->
         <p class="text-xs font-medium text-cyan-400 uppercase tracking-widest bg-cyan-500/10 w-fit px-2 py-0.5 rounded">
            {{ product.categoryName || 'Sản phẩm' }}
         </p>

         <!-- Name -->
         <h3 class="font-display font-semibold text-white text-lg leading-tight line-clamp-2 group-hover:text-[#00f2ea] transition-colors">
            {{ product.name }}
         </h3>

         <!-- Price -->
         <div class="mt-auto pt-2 border-t border-white/10 flex items-center justify-between">
            <span class="text-xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-white to-slate-400">
                {{ formatCurrency(product.basePrice) }}
            </span>
            
            <div class="w-8 h-8 rounded-full bg-white/5 flex items-center justify-center text-white/50 group-hover:bg-[#00f2ea] group-hover:text-black transition-all">
                <Eye :size="16" />
            </div>
         </div>
      </div>
    </div>
  </router-link>
</template>
