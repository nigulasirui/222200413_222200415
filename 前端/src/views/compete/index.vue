<template>
  <div class="container">
    <div>
      
    </div>
    <div class="header">
      <h1>1/4决赛</h1>
      <h1>半决赛</h1>
      <h1>决赛</h1>
    </div>
    <div class="content">
      <div class="col"></div>
      <div class="col"></div>
      <div class="col"></div>
    </div>
  </div>
</template>

<script setup lang="ts">

</script>

<style scoped>
.container{
  width: 90%;
  margin: 0 auto;
  box-sizing: border-box;
}

.header{
  display: flex;
  width: 100%;
  height: 8vh;
  color: white;
  align-items: center;
  justify-content: space-around;
  background: linear-gradient(to right, #012C70, #0092D6,#009CE0);
  margin-bottom: 3vh;
  h1{
    display: block;
    font-size: 3vh;
  }
}

.content{
  display: flex;
  justify-content: space-between;
}

.col{
  width: 30%;
  height: 70vh;
  background-color: antiquewhite;
}
</style>
