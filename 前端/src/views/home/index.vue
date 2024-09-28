<template>
<!--  轮播图-->
  <div class="carousel">
    <el-carousel :interval="3000" height="auto">
      <el-carousel-item v-for="(item, index) in swiperImages" :key="index" style=" height: 500px; !important;">
        <img :src="item" alt="" class="carousel-image"/>
      </el-carousel-item>
    </el-carousel>
  </div>
<!--  奖牌榜-->
  <div class="medals">
    <el-container>
      <el-header>
        <h2>奖牌</h2>
      </el-header>

      <div class="container">
        <div class="bar">
          <div class="head" style="color: white;align-items: center">
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
            <div style="width: 10px">{{ item.rank }}</div>
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
            <div class="item">{{item.gold}}</div>
            <div class="item">{{item.silver}}</div>
            <div class="item">{{item.bronze}}</div>
            <div class="item">{{item.total}}</div>
          </div>
        </div>
        <el-button
          class="all-btn"
          @click="Go('/rankings')"
        >
          <div>
            <p>查看全部</p>
            <img src="@/assets/rankingImg/all-arrow.png" alt="">
          </div>
        </el-button>
      </div>

    </el-container>
  </div>

</template>

<script setup lang="ts">
import swiper1 from '@/assets/img/carousel_img1.png';
import swiper2 from '@/assets/img/carousel_img2.png';
import swiper3 from '@/assets/img/carousel_img3.png';
import swiper4 from '@/assets/img/carousel_img4.png';
import swiper5 from '@/assets/img/carousel_img5.png';
import gold from '@/assets/rankingImg/gold.png'
import silver from '@/assets/rankingImg/silver.png'
import bronze from '@/assets/rankingImg/bronze.png'
import total from '@/assets/rankingImg/total.png'
import { onMounted, ref } from 'vue'
import router from '@/router'
import { useMedalStore } from '@/stores/medals';

const swiperImages = [ swiper1, swiper2, swiper3, swiper4, swiper5 ];
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
  await medalStore.fetchMedals()
  // data.value = medalStore.medals
  data.value = medalStore.medals.slice(0, 7);
  console.log('home');
})

function Go(path: string): void {
  router.push(path)
}

</script>

<style scoped>
.carousel{
  width: 100%;
  margin: 0 auto;
}

.carousel-image {
  width: 100%;
  height: 100%;
  object-fit: cover;
}

.medals {
  width: 80%;
  margin: 10vh auto;
}

.el-header{
  height: fit-content;
  background-image: url(@/assets/img/bg-home-rank.png);
  background-size: cover;
  background-repeat: no-repeat;
  box-sizing: border-box;
  color: white;
}

.el-row {
  margin-bottom: 20px;
}
.el-row:last-child {
  margin-bottom: 0;
}
.el-col {
  border-radius: 4px;
}

.container{
  height: fit-content;
  box-sizing: border-box;
  padding-top: 50px;
  background: linear-gradient(to right, #710D49, #DA4453);
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
  align-items: center;
  justify-content: space-around;
  img{
    display: block;
    height: 35px;
  }
}

.item{
  height: 35px;
  margin: 0 3vw;
}

.all-btn{
  height: 50px;
  width: 100px;
  box-sizing: border-box;
  display: flex;
  float: right;
  justify-content: space-between;
  margin: 50px 10% 50px 0;
  color:black;
  font-weight: bold;
  border: black solid 2px;
  div{
    display: flex;
    align-items: center;
  }
  img{
    height: 50px;
    margin-left: 5px;
  }
}

.all-btn:hover{
  background-color: black;
  color: white;
}
</style>
