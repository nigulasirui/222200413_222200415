<template>
  <h1
    style="margin: 3vh auto;font-size: 30px"
  >
    奖牌榜
  </h1>

  <div class="container">
    <div class="bar">
      <div class="head" style="color: black;align-items: center">
        <div>#</div>
        <div style="margin: 0 3vw">代表队/NOC</div>
      </div>
      <div class="data">
        <div v-for="(item,index) in medalImage" :key="index" class="item">
          <img :src="item" alt=""/>
        </div>
      </div>
    </div>
    <div
      class="bar bar-bg"
      v-for="(item, index) in data" :key="index"
    >
      <div class="head">
        <div>{{ item.rank }}</div>
        <div style="margin: 0 3vw;display: flex;align-items: center;">
          <img
            :src="'https://olympics.com/OG2024/assets/images/flags/OG2024/' + item.organisation + '.webp'"
            alt=""
            style="height: 25px;margin-right: 10px"
          >
          <p>{{item.description}}</p>
        </div>
      </div>
      <div class="data">
        <p class="item">{{item.gold}}</p>
        <p class="item">{{item.silver}}</p>
        <p class="item">{{item.bronze}}</p>
        <p class="item">{{item.total}}</p>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import gold from '@/assets/rankingImg/gold.png'
import silver from '@/assets/rankingImg/silver.png'
import bronze from '@/assets/rankingImg/bronze.png'
import total from '@/assets/rankingImg/total.png'
import { onMounted, ref } from 'vue'
import { useMedalStore } from '@/stores/medals'

const medalImage = [ gold,silver,bronze,total ];

interface Medal {
  organisation: string;
  description: string;
  rank: number;
  gold: number;
  silver: number;
  bronze: number;
  total: number;
}
const data = ref<Medal[]>([]);
const medalStore = useMedalStore();

onMounted(async () => {
  await medalStore.getMedals()
  data.value = medalStore.medals
})
</script>

<style scoped>
.container{
  height: fit-content;
  box-sizing: border-box;
}

.bar{
  display: flex;
  height: 45px;
  width: 90%;
  box-sizing: border-box;
  margin: 0 auto 10px;
  padding: 0 5vw;
  border-radius: 10px;
  justify-content: space-between;
  align-items: baseline;
}

.bar-bg{
  background-color: white;
  border: silver solid 1px;
}

.bar-bg:last-child{
  margin-bottom: 50px;
}

.head{
  display: flex;
  align-items: center;
  justify-content: space-around;
}

.data{
  display: flex;
  width: 60%;
  align-items: baseline;
  justify-content: space-around;
  img{
    display: block;
    height: 35px;
  }
}

.item{
  height: 35px;
  text-align: center;
  vertical-align: middle;
  margin: 0 3vw;
}
</style>
