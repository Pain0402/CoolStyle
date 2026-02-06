<script setup lang="ts">
import { ref, onMounted, computed } from 'vue';
import MainLayout from '../layouts/MainLayout.vue';
import ProductCard from '../components/ProductCard.vue';
import apiClient from '../utils/api';
import { Filter, ChevronDown, SlidersHorizontal } from 'lucide-vue-next';

const products = ref<any[]>([]);
const loading = ref(true);
const sortBy = ref('newest');
const showMobileFilters = ref(false);

const fetchProducts = async () => {
    try {
        const response: any = await apiClient.get('/products');
        products.value = response as any[];
    } catch (err) {
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const sortedProducts = computed(() => {
    // Basic client-side sort for demo
    let p = [...products.value];
    if (sortBy.value === 'price-asc') return p.sort((a,b) => a.basePrice - b.basePrice);
    if (sortBy.value === 'price-desc') return p.sort((a,b) => b.basePrice - a.basePrice);
    return p; // Default (Backend order usually)
});

onMounted(() => {
    fetchProducts();
});
</script>

<template>
  <MainLayout>
    <div class="bg-[#050505] min-h-screen pb-20">
        <!-- Header -->
        <div class="pt-12 pb-8 px-6 container mx-auto">
            <h1 class="font-display text-4xl md:text-6xl font-black text-white mb-4">SHOP ALL</h1>
            <p class="text-gray-400 max-w-xl">Khám phá bộ sưu tập đầy đủ của CoolStyle. Từ Streetwear đến Essentials.</p>
        </div>

        <div class="container mx-auto px-6 grid grid-cols-1 lg:grid-cols-12 gap-10">
            <!-- Sidebar (Desktop) -->
            <div class="hidden lg:block lg:col-span-3 space-y-8">
                <div class="glass p-6 rounded-2xl sticky top-24">
                    <div class="flex items-center gap-2 mb-6 text-white font-bold uppercase tracking-widest text-sm">
                        <Filter :size="16" /> Filters
                    </div>
                    
                    <!-- Categories -->
                    <div class="mb-8">
                        <h4 class="text-gray-500 text-xs font-bold uppercase mb-4">Category</h4>
                        <ul class="space-y-3">
                            <li v-for="cat in ['All Products', 'T-Shirts', 'Hoodies', 'Pants', 'Accessories']" :key="cat">
                                <label class="flex items-center gap-3 cursor-pointer group">
                                    <div class="w-4 h-4 rounded border border-white/20 flex items-center justify-center group-hover:border-cyan-500 transition">
                                        <!-- Fake check -->
                                    </div>
                                    <span class="text-gray-400 group-hover:text-white transition">{{ cat }}</span>
                                </label>
                            </li>
                        </ul>
                    </div>

                    <!-- Price Range -->
                    <div>
                        <h4 class="text-gray-500 text-xs font-bold uppercase mb-4">Price Range</h4>
                        <div class="h-1 bg-white/10 rounded-full overflow-hidden">
                            <div class="h-full bg-cyan-500 w-1/2"></div>
                        </div>
                        <div class="flex justify-between mt-2 text-xs text-gray-400">
                            <span>0đ</span>
                            <span>2.000.000đ</span>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Product Grid -->
            <div class="col-span-1 lg:col-span-9">
                <!-- Toolbar -->
                <div class="flex flex-wrap items-center justify-between mb-8 gap-4">
                    <button @click="showMobileFilters = !showMobileFilters" class="lg:hidden btn-ghost flex items-center gap-2 text-sm py-2 px-4">
                        <SlidersHorizontal :size="16" /> Filters
                    </button>

                    <p class="text-gray-400 text-sm hidden md:block">Showing <span class="text-white font-bold">{{ products.length }}</span> products</p>

                    <div class="relative group">
                        <button class="flex items-center gap-2 text-white bg-white/5 border border-white/10 px-4 py-2 rounded-lg hover:border-white/30 transition text-sm">
                            Sort by: <span class="text-cyan-400 font-bold capitalize">{{ sortBy.replace('-', ' ') }}</span> <ChevronDown :size="14"/>
                        </button>
                        <!-- Dropdown -->
                        <div class="absolute right-0 mt-2 w-48 glass rounded-xl py-2 hidden group-hover:block z-20">
                            <button @click="sortBy = 'newest'" class="block w-full text-left px-4 py-2 hover:bg-white/10 text-sm text-gray-300">Newest</button>
                            <button @click="sortBy = 'price-asc'" class="block w-full text-left px-4 py-2 hover:bg-white/10 text-sm text-gray-300">Price: Low to High</button>
                            <button @click="sortBy = 'price-desc'" class="block w-full text-left px-4 py-2 hover:bg-white/10 text-sm text-gray-300">Price: High to Low</button>
                        </div>
                    </div>
                </div>

                <!-- Grid -->
                 <div v-if="loading" class="grid grid-cols-2 md:grid-cols-3 gap-6">
                    <div v-for="i in 6" :key="i" class="aspect-[3/4] bg-white/5 rounded-2xl animate-pulse"></div>
                 </div>

                 <div v-else class="grid grid-cols-2 md:grid-cols-3 gap-x-6 gap-y-10">
                     <ProductCard v-for="product in sortedProducts" :key="product.id" :product="product" />
                 </div>
            </div>
        </div>
    </div>
  </MainLayout>
</template>
