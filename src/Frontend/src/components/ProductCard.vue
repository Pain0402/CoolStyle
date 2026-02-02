<script setup lang="ts">
import { useCartStore } from '../stores/cart';
import { useToast } from 'vue-toastification';

interface Product {
  id: number;
  name: string;
  slug: string;
  basePrice: number;
  thumbnailUrl: string;
  categoryName: string;
}

const props = defineProps<{ product: Product }>();
const cartStore = useCartStore();
const toast = useToast();

const addToCart = () => {
    cartStore.addToCart(props.product);
    toast.success(`Đã thêm "${props.product.name}" vào giỏ hàng!`);
};

const formatCurrency = (value: number) => {
  return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};
</script>

<template>
  <div class="group cursor-pointer">
    <div class="h-96 bg-gray-100 rounded-none overflow-hidden relative mb-4">
       <!-- Image -->
       <img 
        :src="product.thumbnailUrl || 'https://placehold.co/400x600?text=No+Image'" 
        alt="Product" 
        class="w-full h-full object-cover transition duration-700 group-hover:scale-105"
      />
       
       <!-- Overlay -->
       <div class="absolute inset-0 bg-black/5 opacity-0 group-hover:opacity-100 transition duration-300 pointer-events-none"></div>

       <!-- Action Button (Quick View) - Appears on Hover -->
       <div class="absolute bottom-4 left-4 right-4 translate-y-4 opacity-0 group-hover:translate-y-0 group-hover:opacity-100 transition duration-300">
          <button @click.prevent="addToCart" class="w-full bg-white text-black font-medium py-3 rounded shadow-lg hover:bg-black hover:text-white transition">
            Thêm vào giỏ
          </button>
       </div>
    </div>
    
    <div class="space-y-1">
        <p class="text-xs text-gray-500 uppercase tracking-widest">{{ product.categoryName }}</p>
        <h3 class="font-medium text-lg leading-tight group-hover:underline decoration-1 underline-offset-4">{{ product.name }}</h3>
        <p class="text-gray-900 font-semibold">{{ formatCurrency(product.basePrice) }}</p>
    </div>
  </div>
</template>
