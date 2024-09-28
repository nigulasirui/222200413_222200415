<template>
  <div
    class="navigation"
    :class="[navClass, { 'has-date-picker': route.path === '/dayResult' }]"
  >
    <!--    :class="navClass"-->
    <el-button
      :class="{ active: route.path === '/' }"
      round
      @click="Go('/home')"
    >首页</el-button>
    <el-button
      :class="{ active: route.path === '/dayResult' }"
      round
      @click="Go('/dayResult')"
    >每日赛程</el-button>
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
    <el-date-picker
      v-if="route.path === '/dayResult'"
      v-model="dateValue"
      type="date"
      placeholder="选择日期"
      :editable="false"
      :disabled-date="disabledDate"
      @change="handleDateChange"
      class="date-picker"
    />
  </div>
</template>

<script setup lang="ts">
import router from '@/router'
import { ref, watch } from 'vue'
import { useRoute } from 'vue-router'
import { useDayResultStore } from '@/stores/dayResult'

const route = useRoute()
function Go(path: string): void {
  router.push(path)
}

const navClass = ref('bg-home')
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

const dateValue = ref(new Date('2024-07-24'))
const disabledDate = (time:Date) => {
  const start = new Date('2024-07-23').getTime()
  const end = new Date('2024-08-12').getTime()
  return time.getTime() < start || time.getTime() > end
}

const dayResult = useDayResultStore()
const handleDateChange = () => {
  // 更新store  date
  dayResult.date=dateValue.value.getFullYear() + '-' + dateValue.value.toLocaleDateString('en-US', {
    month: '2-digit',
    day: '2-digit'
  }).replace(',', '').replace('/', '-')
  console.log(dayResult.date)
  //获取新日期赛程
  dayResult.fetchDayResults(dayResult.date)
}
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

.navigation.has-date-picker > *:last-child {
  margin-left: auto;
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
