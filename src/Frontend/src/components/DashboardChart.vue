<script setup lang="ts">
import { ref, onMounted } from 'vue';
import { Chart as ChartJS, CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler } from 'chart.js';
import { Line } from 'vue-chartjs';

ChartJS.register(CategoryScale, LinearScale, PointElement, LineElement, Title, Tooltip, Legend, Filler);

const props = defineProps<{
    chartData: any[]; // Array of { Date: string, Revenue: number }
}>();

const chartOptions = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
        legend: {
            display: false,
        },
        tooltip: {
            callbacks: {
                label: function(context: any) {
                    let label = context.dataset.label || '';
                    if (label) {
                        label += ': ';
                    }
                    if (context.parsed.y !== null) {
                        label += new Intl.NumberFormat('vi-VN', { style: 'currency', currency: 'VND' }).format(context.parsed.y);
                    }
                    return label;
                }
            }
        }
    },
    scales: {
        y: {
            beginAtZero: true,
            ticks: {
                callback: function(value: any) {
                    return new Intl.NumberFormat('vi-VN', { notation: "compact", compactDisplay: "short" }).format(value);
                }
            }
        }
    }
};

const formattedData = ref({
    labels: [] as string[],
    datasets: [
        {
            label: 'Doanh thu',
            backgroundColor: 'rgba(0, 242, 234, 0.2)',
            borderColor: 'rgba(0, 242, 234, 1)',
            borderWidth: 2,
            pointBackgroundColor: 'rgba(0, 242, 234, 1)',
            fill: true,
            data: [] as number[],
            tension: 0.4
        }
    ]
});

onMounted(() => {
    if (props.chartData && props.chartData.length > 0) {
        // Prepare chart data
        const sortedData = [...props.chartData].sort((a, b) => new Date(a.Date).getTime() - new Date(b.Date).getTime());
        
        formattedData.value.labels = sortedData.map(item => {
            const date = new Date(item.Date);
            return `${date.getDate()}/${date.getMonth() + 1}`;
        });
        
        formattedData.value.datasets[0].data = sortedData.map(item => item.Revenue);
    }
});

</script>

<template>
  <div class="h-[300px] w-full">
    <Line v-if="formattedData.labels.length > 0" :data="formattedData" :options="chartOptions" />
  </div>
</template>
