<template>
  <div class="container">
    <div class="date">
      <h1 @click="console.log(dayResult.date);">{{ dayResult.date.split('-')[1]+'月'+dayResult.date.split('-')[2]+'日' }}</h1>
<!--      <h1>{{ formattedDate }}</h1>-->
    </div>
    <div class="matches">
      <div class="item">

      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useDayResultStore } from '@/stores/dayResult'
import { computed, ref, watch } from 'vue'
const dayResult = useDayResultStore()

// const date = ref(dayResult.date.split('-')[1]+'月'+dayResult.date.split('-')[2]+'日')

</script>

<style scoped>
.container{
  width: 90%;
  margin: 0 auto;
  box-sizing: border-box;
  background: linear-gradient(to right, #012C70, #0092D6,#009CE0);
}

.date{
  margin-left: 3vw;
  color: white;
}

.matches{
  width: 100%;
}

.item{
  width: 100%;
}
</style>
