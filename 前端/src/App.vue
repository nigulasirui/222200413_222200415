<script setup lang="ts">
import navigation from './components/navigation.vue';
</script>

<template>
  <el-container>
    <el-header>
      <navigation />
    </el-header>

    <RouterView style="margin-top: 6vh"/>
<!--    <el-main>-->
<!--      <RouterView />-->
<!--    </el-main>-->
  </el-container>
</template>

<style scoped>
*{
  margin: 0;
  padding: 0;
}
el-container{
  display: flex;
  flex-direction: column;
  min-height: 100vh;
  max-width: 100vw;
}
el-header{
  padding:0;
}
.el-main{
  width: 100%;
  height:100vh;
  margin: 0;
}
</style>
