<template>
  <div class="navigation" :class="navClass">
    <el-button
      :class="{ active: route.path === '/' }"
      round
      @click="Go('/home')"
    >首页</el-button>
    <el-button
      :class="{ active: route.path === '/dayResult' }"
      round
      @click="Go('/dayResult')"
    >结果</el-button>
    <el-button
      :class="{ active: route.path === '/rankings' }"
      round
      @click="Go('/rankings')"
    >奖牌</el-button>
    <el-button
      :class="{ active: route.path === '/compete' }"
      round
      @click="Go('/compete')"
    >对阵表</el-button>
  </div>
</template>

<script setup lang="ts">
import router from '@/router'
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'

const route = useRoute()

function Go(path: string): void {
  router.push(path)
}

// 动态背景颜色的 class
const navClass = ref('bg-home')

// 根据路径改变背景
const updateNavClass = (path: string) => {
  if (path === '/home' || path === '/') {
    navClass.value = 'bg-home'
  } else {
    navClass.value = 'bg-default'
  }
}

watch(
  () => route.path,
  (newPath) => {
    updateNavClass(newPath)
  },
  { immediate: true } // 初始加载时也会调用
)
</script>

<style scoped>
.navigation {
  width: 100%;
  margin: 0;
  display: flex;
  padding: 3vh;
  box-sizing: border-box;
  position: fixed;
  top: 0;
  left: 0;
  z-index: 1000;
}

.el-button {
  border: 2px solid #181818;
  background-color: #fff;
  color: #181818;
  transition: background-color 0.3s ease, color 0.3s ease;
}

.el-button:hover,
.el-button.active {
  background-color: #181818;
  color: #fff;
}

.bg-home {
  background-image: url(@/assets/img/bg-home.png);
  background-size: cover;
  background-repeat: no-repeat;
}

.bg-default {
  background-image: url(@/assets/img/bg-default.png);
  background-size: cover;
  background-repeat: no-repeat;
}
</style>
