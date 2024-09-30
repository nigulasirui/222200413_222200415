<template>
  <div class="container">
    <div class="detail">
      <h1 style="font-size: 30px;text-align: center">{{ route.query.disciplineName }}</h1>
      <h1 style="text-align: center">全场比赛结束</h1>
      <div class="compete">
        <div class="competitor">
          <img
            :src="'https://olympics.com/OG2024/assets/images/flags/OG2024/' + competeDetailStore.selected.competitor1.countryEN + '.webp'"
            alt=""
            style="height: 50px;margin-right: 10px;border: silver solid 1px;"
          >
          <p>{{ competeDetailStore.selected.competitor1.name }}</p>
        </div>
        <div class="score">
          <h1>{{ competeDetailStore.selected.competitor1.score }}</h1>
          <h2 style="margin:10px 20px">-</h2>
          <h1>{{ competeDetailStore.selected.competitor2.score }}</h1>
        </div>
        <div class="competitor" style="justify-content: end">
          <p>{{ competeDetailStore.selected.competitor2.name }}</p>
          <img
            :src="'https://olympics.com/OG2024/assets/images/flags/OG2024/' + competeDetailStore.selected.competitor2.countryEN + '.webp'"
            alt=""
            style="height: 50px;margin-left: 10px;border: silver solid 1px;"
          >
        </div>
      </div>
      <h3>{{ competeDetailStore.selected.startDate }}</h3>
    </div>

    <div class="selected">

      <div class="group">
        <el-button
          type="info"
          plain
          v-for="(item,index) in competeDetailStore.competeDetail"
          :key="index"
          :class="{ active: arraysEqual(item.results,competeDetailStore.selectResult) }"
          @click="handleGroup(item)"
        >
          {{ item.stateName }}
        </el-button>
      </div>

      <div class="group-item">
        <div
          class="item"
          v-for="(item,index) in competeDetailStore.selectResult"
          :key="index"
          :class="{ active: itemEqual(item, competeDetailStore.selected) }"
          @click="handleItem(item)"
        >
          <div
            style="border-bottom: black solid 1px;padding-left: 10%;box-sizing: border-box;"
          >
            <h3>{{ item.startDate }}</h3>
          </div>
          <div class="group-competitor">
            <div class="nation">
              <img
                :src="'https://olympics.com/OG2024/assets/images/flags/OG2024/' + item.competitor1.countryEN + '.webp'"
                alt=""
                style="height: 25px;margin-right: 10px;border: silver solid 1px;"
              >
              <p>{{ item.competitor1.name }}</p>
            </div>
            <p>{{ item.competitor1.score ? item.competitor1.score : '' }}</p>
          </div>
          <div class="group-competitor">
            <div class="nation">
              <img
                :src="'https://olympics.com/OG2024/assets/images/flags/OG2024/' + item.competitor2.countryEN + '.webp'"
                alt=""
                style="height: 25px;margin-right: 10px;border: silver solid 1px;"
              >
              <p>{{ item.competitor2.name }}</p>
            </div>
            <p>{{ item.competitor2.score ? item.competitor2.score : '' }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useRoute } from 'vue-router'
const route = useRoute()
import { useCompeteDetailStore } from '@/stores/competeDetail'

interface Competitor {
  countryEN: string;
  name: string;
  score: string;
  isWinner: boolean;
}

interface Result {
  startDate: string;
  competitor1: Competitor;
  competitor2: Competitor;
}

interface State {
  stateName: string;
  results: Result[];
}

const competeDetailStore = useCompeteDetailStore()

const handleGroup = (item:State)=>{
  competeDetailStore.selectResult=item.results
  competeDetailStore.selected=item.results[0]
}

const handleItem = (item:Result)=>{
  competeDetailStore.selected=item
}
//button active
const competitorEqual = (comp1: Competitor, comp2: Competitor): boolean => {
  return (
    comp1.countryEN === comp2.countryEN &&
    comp1.name === comp2.name &&
    comp1.score === comp2.score &&
    comp1.isWinner === comp2.isWinner
  );
};

const resultEqual = (result1: Result, result2: Result): boolean => {
  return (
    result1.startDate === result2.startDate &&
    competitorEqual(result1.competitor1, result2.competitor1) &&
    competitorEqual(result1.competitor2, result2.competitor2)
  );
};

const arraysEqual = (arr1: Result[], arr2: Result[]): boolean => {
  if (arr1.length !== arr2.length) {
    return false;
  }

  for (let i = 0; i < arr1.length; i++) {
    if (!resultEqual(arr1[i], arr2[i])) {
      return false;
    }
  }

  return true;
};

//item active
const itemEqual=(obj1: any, obj2: any): boolean =>{
  if (obj1 === obj2) {
    return true;
  }

  const keys1 = Object.keys(obj1);
  const keys2 = Object.keys(obj2);

  if (keys1.length !== keys2.length) {
    return false;
  }

  for (const key of keys1) {
    if (obj1[key] !== obj2[key]) {
      return false;
    }
  }
  return true;
}

</script>

<style scoped>
.container {
  width: 90%;
  margin: 0 auto;
}

.detail{
  width: 100%;
  height: 35vh;
  box-sizing: border-box;
  padding: 5%;
  margin-bottom: 5vh;
  background: linear-gradient(to right, #012C70, #0092D6,#009CE0);
  color: white;
}

.detail>h1{
  margin: 10px auto;
}

.compete{
  width: 100%;
  display: flex;
  justify-content: space-between;
}

.competitor{
  width: 60%;
  display: flex;
  align-items: center;
  p{
    font-size: 20px;
    font-weight: bold;
  }
}

.score{
  width: 30%;
  display: flex;
  align-items: center;
  justify-content: center;
  h1{
    font-size: 30px;
  }
}

.group{
  width: 100%;
  height: 50px;
  display: flex;
  margin-bottom: 3vh;
  overflow: scroll;
  overflow-y: hidden;
  overflow-x: auto;
}

.group .el-button{
  background-color: #ffffff;
  color: black;
}

.active,
.group .el-button:hover{
  background-color: black;
  color: white;
}

.group-item{
  width: 100%;
  height: 20vh;
  min-height: 200px;
  display: flex;
  overflow: scroll;
  overflow-y: hidden;
  overflow-x: auto;
  .item{
    width: 30%;
    min-width: 250px;
    height:  185px;
/*    min-height: 185px;*/
    box-sizing: border-box;
    border: black solid 1px;
    border-radius: 10px;
    margin-right: 10px;
  }
}

.group-competitor{
  width: 100%;
  display: flex;
  justify-content: space-between;
  box-sizing: border-box;
  padding: 0 10%;
}

.nation{
  display: flex;
  justify-content: start;
  align-items: center;
  box-sizing: border-box;
  padding-left: 1vw;
}
</style>
