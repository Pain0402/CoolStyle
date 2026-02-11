<script setup lang="ts">
import { ref, onMounted, computed, watch } from 'vue';
import { useRoute, useRouter } from 'vue-router';
import MainLayout from '../layouts/MainLayout.vue';
import ProductCard from '../components/ProductCard.vue';
import apiClient from '../utils/api';
import { Filter, ChevronDown, SlidersHorizontal, X } from 'lucide-vue-next';

const route = useRoute();
const router = useRouter();
const products = ref<any[]>([]);
const loading = ref(true);
const sortBy = ref('newest');
const selectedCategory = ref('All Products');
const showMobileFilters = ref(false);

const searchQuery = ref('');

// Price Filter State
const minPrice = ref(0);
const maxPrice = ref(10000000); // Default max
const priceRange = ref<[number, number]>([0, 10000000]); // [min, max] selected

const formatCurrency = (value: number) => {
    return new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(value);
};

const fetchProducts = async () => {
    try {
        const response: any = await apiClient.get('/products');
        products.value = response as any[];
        
        // Calculate min and max prices from products
        if (products.value.length > 0) {
            const prices = products.value.map(p => p.basePrice || 0);
            minPrice.value = Math.min(...prices);
            maxPrice.value = Math.max(...prices);
            
            // Initialize filter range if not already set or invalid
             if (priceRange.value[1] === 10000000) {
                 priceRange.value = [minPrice.value, maxPrice.value];
             }
        }

    } catch (err) {
        console.error(err);
    } finally {
        loading.value = false;
    }
};

const availableCategories = computed(() => {
    const cats = new Set(products.value.map(p => p.categoryName).filter(Boolean));
    return ['All Products', ...Array.from(cats)];
});

const filteredProducts = computed(() => {
    let p = [...products.value];

    // Filter by Category
    if (selectedCategory.value && selectedCategory.value !== 'All Products') {
        p = p.filter(prod => 
            prod.categoryName?.toLowerCase() === selectedCategory.value.toLowerCase() || 
            prod.categoryName?.toLowerCase().includes(selectedCategory.value.toLowerCase()) || 
            prod.name.toLowerCase().includes(selectedCategory.value.toLowerCase())
        );
    }

    // Filter by Search Query
    if (searchQuery.value) {
        p = p.filter(prod => 
            prod.name.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
            prod.description?.toLowerCase().includes(searchQuery.value.toLowerCase()) ||
            prod.categoryName?.toLowerCase().includes(searchQuery.value.toLowerCase())
        );
    }

    // Filter by Price
    p = p.filter(prod => {
        const price = prod.basePrice || 0;
        return price >= priceRange.value[0] && price <= priceRange.value[1];
    });

    // Sort
    if (sortBy.value === 'newest') {
        // Assuming there is a createdAt or similar, otherwise default order
        // if (p[0]?.createdAt) return p.sort((a,b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
        return p.reverse(); // Simple reverse for now as usually latest are last or first depends on API
    }
    if (sortBy.value === 'price-asc') return p.sort((a,b) => (a.basePrice || 0) - (b.basePrice || 0));
    if (sortBy.value === 'price-desc') return p.sort((a,b) => (b.basePrice || 0) - (a.basePrice || 0));
    
    return p;
});

const selectCategory = (cat: string) => {
    selectedCategory.value = cat;
    // Clear search when category changes for cleaner experience
    searchQuery.value = '';
    router.push({ query: { ...route.query, category: cat === 'All Products' ? undefined : cat } });
};

const clearFilters = () => {
    selectedCategory.value = 'All Products';
    searchQuery.value = '';
    priceRange.value = [minPrice.value, maxPrice.value];
    router.push({ query: {} });
};

watch(() => route.query.category, (newCat) => {
    if (newCat) {
        selectedCategory.value = newCat as string;
    } else {
        selectedCategory.value = 'All Products';
    }
}, { immediate: true });

watch(() => route.query.search, (newSearch) => {
    searchQuery.value = (newSearch as string) || '';
}, { immediate: true });

onMounted(() => {
    fetchProducts();
});
</script>

<template>
  <MainLayout>
    <div class="bg-[#050505] min-h-screen pb-20 font-sans">
        <!-- Header -->
        <div class="pt-12 pb-8 px-6 container mx-auto">
            <h1 class="font-display text-4xl md:text-6xl font-black text-white mb-4 uppercase tracking-tight">
                <template v-if="searchQuery">SEARCH: {{ searchQuery }}</template>
                <template v-else>{{ selectedCategory === 'All Products' ? 'SHOP ALL' : selectedCategory }}</template>
            </h1>
            <p class="text-gray-400 max-w-xl text-lg">
                <template v-if="searchQuery">Showing {{ filteredProducts.length }} results for your search.</template>
                <template v-else>Khám phá bộ sưu tập đầy đủ của CoolStyle. Từ Streetwear đến Essentials.</template>
            </p>
        </div>

        <div class="container mx-auto px-6 grid grid-cols-1 lg:grid-cols-12 gap-10">
            <!-- Sidebar (Desktop) -->
            <div class="hidden lg:block lg:col-span-3 space-y-8">
                <div class="glass p-6 rounded-2xl sticky top-24 border border-white/5 bg-white/5 backdrop-blur-xl">
                    <div class="flex items-center justify-between mb-6">
                        <div class="flex items-center gap-2 text-white font-bold uppercase tracking-widest text-sm">
                            <Filter :size="16" /> Filters
                        </div>
                        <button v-if="selectedCategory !== 'All Products' || priceRange[0] > minPrice || priceRange[1] < maxPrice" 
                                @click="clearFilters" 
                                class="text-xs text-cyan-400 hover:text-cyan-300 transition cursor-pointer">
                            Clear All
                        </button>
                    </div>
                    
                    <!-- Categories -->
                    <div class="mb-8">
                        <h4 class="text-gray-500 text-xs font-bold uppercase mb-4 tracking-wider">Category</h4>
                        <ul class="space-y-3">
                            <li v-for="cat in availableCategories" :key="cat">
                                <button @click="selectCategory(cat)" class="flex items-center gap-3 cursor-pointer group w-full text-left">
                                    <div :class="['w-4 h-4 rounded border flex items-center justify-center transition-all duration-300', selectedCategory === cat ? 'bg-cyan-500 border-cyan-500 shadow-[0_0_10px_rgba(6,182,212,0.5)]' : 'border-white/20 group-hover:border-cyan-500']">
                                        <div v-if="selectedCategory === cat" class="w-1.5 h-1.5 bg-black rounded-sm"></div>
                                    </div>
                                    <span :class="['transition-colors duration-300 text-sm', selectedCategory === cat ? 'text-white font-bold' : 'text-gray-400 group-hover:text-white']">{{ cat }}</span>
                                </button>
                            </li>
                        </ul>
                    </div>

                    <!-- Price Range -->
                    <div>
                        <div class="flex items-center justify-between mb-4">
                             <h4 class="text-gray-500 text-xs font-bold uppercase tracking-wider">Price Range</h4>
                             <span class="text-xs text-cyan-400">{{ formatCurrency(priceRange[0]) }} - {{ formatCurrency(priceRange[1]) }}</span>
                        </div>
                       
                        <div class="px-2">
                             <input 
                                type="range" 
                                v-model.number="priceRange[1]" 
                                :min="minPrice" 
                                :max="maxPrice" 
                                step="10000"
                                class="w-full accent-cyan-500 cursor-pointer h-1 bg-white/10 rounded-lg appearance-none"
                            />
                             <div class="flex justify-between mt-3 text-[10px] text-gray-500 font-mono">
                                <span>{{ formatCurrency(minPrice) }}</span>
                                <span>{{ formatCurrency(maxPrice) }}</span>
                            </div>
                        </div>
                        
                         <div class="mt-4 grid grid-cols-2 gap-2">
                            <div>
                                <label class="text-[10px] text-gray-500 uppercase block mb-1">Min</label>
                                <input type="number" v-model.number="priceRange[0]" :min="minPrice" :max="priceRange[1]" class="w-full bg-white/5 border border-white/10 rounded px-2 py-1 text-xs text-white focus:outline-none focus:border-cyan-500 transition" />
                            </div>
                             <div>
                                <label class="text-[10px] text-gray-500 uppercase block mb-1">Max</label>
                                <input type="number" v-model.number="priceRange[1]" :min="priceRange[0]" :max="maxPrice" class="w-full bg-white/5 border border-white/10 rounded px-2 py-1 text-xs text-white focus:outline-none focus:border-cyan-500 transition" />
                            </div>
                        </div>
                    </div>
                </div>
            </div>

            <!-- Product Grid -->
            <div class="col-span-1 lg:col-span-9">
                <!-- Toolbar -->
                <div class="flex flex-wrap items-center justify-between mb-8 gap-4 bg-white/5 p-4 rounded-xl border border-white/5 backdrop-blur-sm">
                    <button @click="showMobileFilters = !showMobileFilters" class="lg:hidden btn-ghost flex items-center gap-2 text-sm py-2 px-4 hover:bg-white/10 rounded-lg transition text-white">
                        <SlidersHorizontal :size="16" /> Filters
                    </button>

                    <p class="text-gray-400 text-sm hidden md:block">Showing <span class="text-white font-bold">{{ filteredProducts.length }}</span> products</p>

                    <div class="relative group ml-auto">
                        <button class="flex items-center gap-2 text-white bg-black/20 border border-white/10 px-4 py-2 rounded-lg hover:border-cyan-500/50 hover:shadow-[0_0_15px_rgba(6,182,212,0.15)] transition text-sm">
                            Sort by: <span class="text-cyan-400 font-bold capitalize">{{ sortBy === 'newest' ? 'Newest' : sortBy === 'price-asc' ? 'Price: Low to High' : 'Price: High to Low' }}</span> <ChevronDown :size="14"/>
                        </button>
                        <!-- Dropdown -->
                        <div class="absolute right-0 mt-2 w-56 glass rounded-xl py-2 hidden group-hover:block z-30 border border-white/10 shadow-xl bg-[#0a0a0a]">
                            <button @click="sortBy = 'newest'" class="block w-full text-left px-4 py-3 hover:bg-white/10 text-sm text-gray-300 transition-colors">Newest Arrivals</button>
                            <button @click="sortBy = 'price-asc'" class="block w-full text-left px-4 py-3 hover:bg-white/10 text-sm text-gray-300 transition-colors">Price: Low to High</button>
                            <button @click="sortBy = 'price-desc'" class="block w-full text-left px-4 py-3 hover:bg-white/10 text-sm text-gray-300 transition-colors">Price: High to Low</button>
                        </div>
                    </div>
                </div>

                <!-- Grid -->
                 <div v-if="loading" class="grid grid-cols-2 md:grid-cols-3 gap-6">
                    <div v-for="i in 6" :key="i" class="aspect-[3/4] bg-white/5 rounded-2xl animate-pulse"></div>
                 </div>

                 <div v-else-if="filteredProducts.length === 0" class="flex flex-col items-center justify-center py-20 text-center">
                    <div class="w-16 h-16 bg-white/5 rounded-full flex items-center justify-center mb-4 text-gray-500">
                        <Filter :size="32" />
                    </div>
                    <h3 class="text-xl font-bold text-white mb-2">No products found</h3>
                    <p class="text-gray-400 mb-6">Try broadening your search or adjusting your filters.</p>
                    <button @click="clearFilters" class="px-6 py-2 bg-cyan-500 hover:bg-cyan-600 text-black font-bold rounded-lg transition shadow-[0_0_20px_rgba(6,182,212,0.4)]">
                        Clear All Filters
                    </button>
                 </div>

                 <div v-else class="grid grid-cols-2 md:grid-cols-3 gap-x-6 gap-y-10">
                     <ProductCard v-for="product in filteredProducts" :key="product.id" :product="product" />
                 </div>
            </div>
        </div>

        <!-- Mobile Filters Modal -->
        <div v-if="showMobileFilters" class="fixed inset-0 z-50 lg:hidden">
            <div class="absolute inset-0 bg-black/80 backdrop-blur-sm" @click="showMobileFilters = false"></div>
            <div class="absolute right-0 top-0 h-full w-80 bg-[#111] overflow-y-auto p-6 shadow-2xl border-l border-white/10">
                <div class="flex items-center justify-between mb-8">
                    <h3 class="text-white font-bold text-lg uppercase tracking-wider">Filters</h3>
                    <button @click="showMobileFilters = false" class="text-gray-400 hover:text-white transition">
                        <X :size="24" />
                    </button>
                </div>

                <!-- Mobile Categories -->
                 <div class="mb-8">
                    <h4 class="text-gray-500 text-xs font-bold uppercase mb-4 tracking-wider">Category</h4>
                    <ul class="space-y-3">
                        <li v-for="cat in availableCategories" :key="cat">
                            <button @click="selectCategory(cat); showMobileFilters = false" class="flex items-center gap-3 w-full text-left">
                                <div :class="['w-4 h-4 rounded border flex items-center justify-center transition', selectedCategory === cat ? 'bg-cyan-500 border-cyan-500' : 'border-white/20']">
                                    <div v-if="selectedCategory === cat" class="w-1.5 h-1.5 bg-black rounded-sm"></div>
                                </div>
                                <span :class="['transition', selectedCategory === cat ? 'text-white font-bold' : 'text-gray-400']">{{ cat }}</span>
                            </button>
                        </li>
                    </ul>
                </div>

                <!-- Mobile Price Range -->
                <div class="mb-8">
                    <h4 class="text-gray-500 text-xs font-bold uppercase mb-4 tracking-wider">Price Range</h4>
                    <input 
                        type="range" 
                        v-model.number="priceRange[1]" 
                        :min="minPrice" 
                        :max="maxPrice" 
                        step="10000"
                        class="w-full accent-cyan-500 cursor-pointer h-1 bg-white/10 rounded-lg appearance-none mb-4"
                    />
                    <div class="grid grid-cols-2 gap-2">
                        <input type="number" v-model.number="priceRange[0]" class="w-full bg-white/5 border border-white/10 rounded px-3 py-2 text-sm text-white" placeholder="Min" />
                        <input type="number" v-model.number="priceRange[1]" class="w-full bg-white/5 border border-white/10 rounded px-3 py-2 text-sm text-white" placeholder="Max" />
                    </div>
                </div>

                <div class="pt-4 border-t border-white/10">
                    <button @click="clearFilters(); showMobileFilters = false" class="w-full py-3 border border-white/10 rounded-lg text-white hover:bg-white/5 transition mb-3 font-bold text-sm">Clear All</button>
                    <button @click="showMobileFilters = false" class="w-full py-3 bg-cyan-500 hover:bg-cyan-400 rounded-lg text-black font-bold transition shadow-[0_0_20px_rgba(6,182,212,0.4)]">View Results</button>
                </div>
            </div>
        </div>
    </div>
  </MainLayout>
</template>
