<script setup lang="ts">
import { ref, onMounted, onUnmounted } from 'vue';
import * as THREE from 'three';

const container = ref<HTMLDivElement | null>(null);

let scene: THREE.Scene;
let camera: THREE.PerspectiveCamera;
let renderer: THREE.WebGLRenderer;
let particles: THREE.Points;
let mouseX = 0;
let mouseY = 0;
let windowHalfX = window.innerWidth / 2;
let windowHalfY = window.innerHeight / 2;

const init = () => {
    if (!container.value) return;

    // Scene
    scene = new THREE.Scene();

    // Camera
    camera = new THREE.PerspectiveCamera(75, container.value.clientWidth / container.value.clientHeight, 1, 10000);
    camera.position.z = 1000;

    // Particles
    const geometry = new THREE.BufferGeometry();
    const count = 2000;
    const positions = new Float32Array(count * 3);
    const colors = new Float32Array(count * 3);

    const color1 = new THREE.Color(0x00f2ea); // Cyan
    const color2 = new THREE.Color(0xff0055); // Purple-ish Red

    for (let i = 0; i < count * 3; i += 3) {
        // Position
        positions[i] = (Math.random() * 2 - 1) * 2000;
        positions[i + 1] = (Math.random() * 2 - 1) * 2000;
        positions[i + 2] = (Math.random() * 2 - 1) * 2000;

        // Color Mix
        const mixedColor = Math.random() > 0.5 ? color1 : color2;
        colors[i] = mixedColor.r;
        colors[i + 1] = mixedColor.g;
        colors[i + 2] = mixedColor.b;
    }

    geometry.setAttribute('position', new THREE.BufferAttribute(positions, 3));
    geometry.setAttribute('color', new THREE.BufferAttribute(colors, 3));

    const material = new THREE.PointsMaterial({
        size: 4,
        vertexColors: true,
        transparent: true,
        opacity: 0.8,
        sizeAttenuation: true
    });

    particles = new THREE.Points(geometry, material);
    scene.add(particles);

    // Renderer
    renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
    renderer.setPixelRatio(window.devicePixelRatio);
    renderer.setSize(container.value.clientWidth, container.value.clientHeight);
    container.value.appendChild(renderer.domElement);

    // Events
    if(!('ontouchstart' in window)) {
        document.addEventListener('mousemove', onDocumentMouseMove);
    }
    window.addEventListener('resize', onWindowResize);
};

const onWindowResize = () => {
    if (!container.value) return;
    windowHalfX = container.value.clientWidth / 2;
    windowHalfY = getHeight() / 2;
    
    camera.aspect = container.value.clientWidth / getHeight();
    camera.updateProjectionMatrix();
    renderer.setSize(container.value.clientWidth, getHeight());
};

const getHeight = () => {
    return container.value ? container.value.clientHeight : window.innerHeight;
}

const onDocumentMouseMove = (event: MouseEvent) => {
    mouseX = (event.clientX - windowHalfX) * 0.5; // Sensitivity
    mouseY = (event.clientY - windowHalfY) * 0.5;
};

const animate = () => {
    if (!container.value) return;
    requestAnimationFrame(animate);
    render();
};

const render = () => {
    // Smooth camera movement towards mouse
    camera.position.x += (mouseX - camera.position.x) * 0.05;
    camera.position.y += (-mouseY - camera.position.y) * 0.05;
    camera.lookAt(scene.position);

    // Constant rotation for "alive" feel
    if (particles) {
        particles.rotation.y += 0.001;
        particles.rotation.x += 0.0005;
    }

    renderer.render(scene, camera);
};

onMounted(() => {
    init();
    animate();
});

onUnmounted(() => {
    window.removeEventListener('resize', onWindowResize);
    document.removeEventListener('mousemove', onDocumentMouseMove);
    // Cleanup Three.js resources to prevent leaks
    if (renderer) renderer.dispose();
    if (particles) {
         particles.geometry.dispose();
         (particles.material as THREE.Material).dispose();
    }
});
</script>

<template>
  <div ref="container" class="absolute inset-0 w-full h-full pointer-events-none z-0 overflow-hidden"></div>
</template>
