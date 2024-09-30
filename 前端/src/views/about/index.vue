<template>
  <div class="container">
    <el-divider content-position="center">
      <h1>关于赛会</h1>
    </el-divider>
    <div class="info">
      <el-card
        style="max-width: 300px"
        class="pointer"
        @click="openLink('https://olympics.com/zh/paris-2024/sports/')"
      >
        <template #header>运动项目</template>
        <img
          src="@/assets/aboutImg/project1.jpg"
          style="width: 100%"
         alt=""/>
      </el-card>
      <el-card
        style="max-width: 300px"
        class="pointer"
        @click="openLink('https://olympics.com/zh/paris-2024/venues')"
      >
        <template #header>场馆</template>
        <img
          src="@/assets/aboutImg/project2.avif"
          style="width: 100%"
          alt=""/>
      </el-card>
      <el-card
        style="max-width: 300px"
        class="pointer"
        @click="openLink('https://olympics.com/zh/paris-2024/videos/list')"
      >
        <template #header>共庆奥运盛会</template>
        <img
          src="@/assets/aboutImg/project3.jpg"
          style="width: 100%"
          alt=""/>
      </el-card>
    </div>

    <el-divider content-position="center" style="margin: 50px 0">
      <h1>下届奥运会</h1>
    </el-divider>
    <div class="next">
      <img src="@/assets/aboutImg/next1.svg" style="width: 20%;" alt="" @click="openLink('https://milanocortina2026.olympics.com/en')">
      <img src="@/assets/aboutImg/next2.svg" style="width: 20%;" alt="" @click="openLink('https://olympics.com/zh/olympic-games/los-angeles-2028')">
      <img src="@/assets/aboutImg/next3.svg" style="width: 20%;" alt="" @click="openLink('https://olympics.com/zh/olympic-games/french-alps-2030')">
      <img src="@/assets/aboutImg/next4.svg" style="width: 20%;" alt="" @click="openLink('https://olympics.com/zh/olympic-games/brisbane-2032')">
    </div>
    <div style="width: 100%;display: flex;justify-content: center;">
      <img src="@/assets/aboutImg/next5.svg" style="width: 20%;" alt="" @click="openLink('https://olympics.com/zh/olympic-games/salt-lake-city-utah-2034')">
    </div>

    <el-divider content-position="center" style="margin: 50px 0">
      <h1>奥林匹克网站</h1>
    </el-divider>
    <div class="Olympic" style="margin-bottom: 100px">
      <img src="@/assets/aboutImg/Olympic1.svg" style="width: 20%;" alt="" @click="openLink('https://olympics.com/ioc')">
      <img src="@/assets/aboutImg/Olympic2.svg" style="width: 20%;" alt="" @click="openLink('https://olympics.com/museum')">
      <img src="@/assets/aboutImg/Olympic3.svg" style="width: 20%;" alt="" @click="openLink('https://olympics.com/athlete365')">
      <img src="@/assets/aboutImg/Olympic4.svg" style="width: 20%;" alt="" @click="openLink('https://olympics.com/en/olympic-refuge-foundation/')">
    </div>
  </div>
</template>

<script setup lang="ts">
function openLink(url: string) {
  window.open(url, '_blank');
}
</script>

<style scoped>
.container{
  width: 85%;
  margin: 0 auto;
}

img,
.pointer{
  cursor: pointer;
}

.info{
  width: 100%;
  margin-top: 50px;
  display: flex;
  justify-content: space-around;
}

.Olympic,
.next{
  width: 100%;
  display: flex;
  justify-content: space-around;
}
</style>
