<script setup lang="ts">
import { useCartStore } from '../stores/cart';
import { useWishlistStore } from '../stores/wishlist';
import { useToast } from 'vue-toastification';
import { ShoppingBag, Heart, Eye } from 'lucide-vue-next';
import { computed } from 'vue';

interface Product {
  id: number;
  name: string;
  slug: string;
  basePrice: number;
  thumbnailUrl: string;
  categoryName: string;
  isNew?: boolean; 
  discount?: number;
}

const props = defineProps<{ product: Product }>();
const cartStore = useCartStore();
const wishlistStore = useWishlistStore();
const toast = useToast();

const isWishlisted = computed(() => wishlistStore.isInWishlist(props.product.id));

const toggleWishlist = async (e: Event) => {
    e.preventDefault();
    e.stopPropagation();
    
    const added = await wishlistStore.toggleWishlist(props.product);
    if (added === undefined) {
         toast.error("Vui lòng đăng nhập để yêu thích sản phẩm!");
         return;
    }
    
    if (added) {
        toast.success("Đã thêm vào danh sách yêu thích!");
    } else {
        toast.info("Đã xóa khỏi danh sách yêu thích");
    }
};

const addToCart = (e: Event) => {
    e.stopPropagation(); 
    cartStore.addToCart(props.product);
    toast.success(`Đã thêm "${props.product.name}" vào giỏ hàng!`);
};

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

const isNew = props.product.id > 300 || props.product.isNew;

const handleImageError = (e: Event) => {
    (e.target as HTMLImageElement).src = 'https://placehold.co/600x800/1a1a1a/FFF?text=CoolStyle';
};
</script>

<template>
  <router-link :to="`/product/${product.slug}`" class="group relative block h-full">
    <!-- Glow Effect Background -->
    <div class="absolute -inset-0.5 bg-gradient-to-r from-theme-primary to-theme-secondary rounded-2xl opacity-0 group-hover:opacity-75 blur transition duration-500 will-change-[opacity]"></div>
    
    <!-- Main Card Content -->
    <div class="relative h-full bg-[#0f1012] rounded-2xl overflow-hidden border border-white/5 flex flex-col transition-transform duration-300 group-hover:-translate-y-1">
      
      <!-- Image Section -->
      <div class="relative aspect-[3/4] overflow-hidden bg-white/5">
        <!-- New Badge -->
        <span v-if="isNew" class="absolute top-3 left-3 z-20 px-3 py-1 text-xs font-bold text-black bg-theme-primary rounded-full shadow-theme-primary">
          NEW
        </span>

        <!-- Wishlist Button -->
        <button 
          @click="toggleWishlist"
          :class="['absolute top-3 right-3 z-20 p-2 rounded-full backdrop-blur-md transition-all duration-300', 
                   isWishlisted ? 'bg-red-500 text-white shadow-[0_0_15px_rgba(239,68,68,0.5)]' : 'text-white/70 bg-white/10 hover:text-red-500']"
        >
           <Heart :size="20" :fill="isWishlisted ? 'currentColor' : 'none'" />
        </button>

        <!-- Product Image -->
        <img 
          :src="product.thumbnailUrl || 'https://placehold.co/600x800/1a1a1a/FFF?text=CoolStyle'" 
          @error="handleImageError"
          alt="Product" 
          loading="lazy"
          class="w-full h-full object-cover transition-transform duration-700 ease-out group-hover:scale-110"
        />

        <!-- Overlay Actions -->
        <div class="absolute inset-0 bg-black/40 backdrop-blur-[2px] opacity-0 group-hover:opacity-100 transition-opacity duration-300 flex flex-col justify-end p-4">
             <button 
                @click="addToCart"
                class="w-full flex items-center justify-center gap-2 bg-white text-black font-bold py-3 rounded-xl hover:bg-theme-primary hover:shadow-theme-primary transition-all duration-300 transform translate-y-4 group-hover:translate-y-0"
             >
                <ShoppingBag :size="18" /> Thêm vào giỏ
             </button>
        </div>
      </div>

      <!-- Content Section -->
      <div class="p-4 flex flex-col flex-1 gap-2">
         <!-- Category -->
         <p class="text-xs font-medium text-theme-primary uppercase tracking-widest bg-white/5 border border-theme-primary/30 w-fit px-2 py-0.5 rounded">
            {{ product.categoryName || 'Sản phẩm' }}
         </p>

         <!-- Name -->
         <h3 class="font-display font-semibold text-white text-lg leading-tight line-clamp-2 group-hover:text-theme-primary transition-colors">
            {{ product.name }}
         </h3>

         <!-- Price -->
         <div class="mt-auto pt-2 border-t border-white/10 flex items-center justify-between">
            <span class="text-xl font-bold bg-clip-text text-transparent bg-gradient-to-r from-white to-slate-400">
                {{ formatCurrency(product.basePrice) }}
            </span>
            
            <div class="w-8 h-8 rounded-full bg-white/5 flex items-center justify-center text-white/50 group-hover:bg-theme-primary group-hover:text-black transition-all">
                <Eye :size="16" />
            </div>
         </div>
      </div>
    </div>
  </router-link>
</template>
