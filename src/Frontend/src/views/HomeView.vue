<script setup lang="ts">
import { ref, onMounted } from 'vue';
import MainLayout from '../layouts/MainLayout.vue';
import ProductCard from '../components/ProductCard.vue';
import apiClient from '../utils/api';

// State
const products = ref<any[]>([]); // Changed from ref([]) to ref<any[]>
const loading = ref(true);
const error = ref('');

// Fetch Data
const fetchProducts = async () => {
    try {
        const response: any = await apiClient.get('/products');
        // Type assertion for `any` since data is unknown at compile time
        products.value = response as any[]; 
    } catch (err) {
        error.value = 'Không thể tải sản phẩm. Vui lòng kiểm tra kết nối.';
        console.error(err);
    } finally {
        loading.value = false;
    }
};

onMounted(() => {
    fetchProducts();
});
</script>

<template>
  <MainLayout>
    <!-- Hero Banner -->
    <div class="bg-gray-100 py-20 px-4 text-center mb-12">
        <h2 class="text-4xl md:text-6xl font-extrabold tracking-tight mb-4">Summer Collection 2026</h2>
        <p class="text-gray-600 text-lg mb-8 max-w-xl mx-auto">Trải nghiệm sự thoải mái tuyệt đối với chất liệu Cotton Compact mới nhất.</p>
        <button class="btn-primary">Mua ngay</button>
    </div>

    <!-- Product Grid -->
    <div class="max-w-7xl mx-auto px-4 pb-20">
      <div class="flex justify-between items-end mb-8">
         <h3 class="text-2xl font-bold">Sản phẩm mới nhất</h3>
         <button class="text-sm font-semibold hover:underline">Xem tất cả</button>
      </div>

      <!-- Loading State -->
      <div v-if="loading" class="grid grid-cols-1 md:grid-cols-4 gap-8">
         <div v-for="i in 4" :key="i" class="h-96 bg-gray-200 animate-pulse rounded"></div>
      </div>

      <!-- Error State -->
      <div v-else-if="error" class="text-center py-12 text-red-500">
         {{ error }}
      </div>

      <!-- Data State -->
      <div v-else class="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 lg:grid-cols-4 gap-x-8 gap-y-12">
        <ProductCard v-for="product in products" :key="product.id" :product="product" />
      </div>
    </div>
  </MainLayout>
</template>
