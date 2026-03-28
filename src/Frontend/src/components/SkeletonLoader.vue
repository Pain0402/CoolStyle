<script setup lang="ts">
defineProps<{
    type?: 'product-card' | 'list-item' | 'text' | 'profile'
    count?: number
}>();
</script>

<template>
    <!-- Product Card Skeleton -->
    <template v-if="type === 'product-card'">
        <div v-for="i in (count ?? 6)" :key="i" class="animate-pulse">
            <div class="aspect-[3/4] bg-white/5 rounded-2xl mb-3 border border-white/5"></div>
            <div class="h-4 bg-white/5 rounded w-3/4 mb-2"></div>
            <div class="h-4 bg-white/5 rounded w-1/2"></div>
        </div>
    </template>

    <!-- List Item Skeleton (for orders, addresses) -->
    <template v-else-if="type === 'list-item'">
        <div v-for="i in (count ?? 4)" :key="i" class="animate-pulse p-4 rounded-xl border border-white/10 bg-white/5 mb-3">
            <div class="flex justify-between items-center">
                <div class="space-y-2 flex-1">
                    <div class="h-4 bg-white/10 rounded w-1/3"></div>
                    <div class="h-3 bg-white/5 rounded w-1/2"></div>
                </div>
                <div class="h-8 w-24 bg-white/10 rounded-lg"></div>
            </div>
        </div>
    </template>

    <!-- Text lines Skeleton -->
    <template v-else-if="type === 'text'">
        <div v-for="i in (count ?? 3)" :key="i"
             :class="['animate-pulse h-4 bg-white/5 rounded mb-2', i === (count ?? 3) ? 'w-2/3' : 'w-full']">
        </div>
    </template>

    <!-- Profile Skeleton -->
    <template v-else-if="type === 'profile'">
        <div class="animate-pulse space-y-4">
            <div class="w-24 h-24 rounded-full bg-white/10 mx-auto"></div>
            <div class="h-5 bg-white/10 rounded w-1/2 mx-auto"></div>
            <div class="h-4 bg-white/5 rounded w-1/3 mx-auto"></div>
        </div>
    </template>

    <!-- Default: single block -->
    <template v-else>
        <div class="animate-pulse h-16 bg-white/5 rounded-xl border border-white/10"></div>
    </template>
</template>
