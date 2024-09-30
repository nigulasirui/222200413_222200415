<template>
  <div class="container">
    <div class="date">
      <h1>{{ dayResultStore.date.split('-')[1]+'月'+dayResultStore.date.split('-')[2]+'日' }}</h1>
    </div>
    <div class="matches">
      <div
        class="item"
        v-for="(item,index) in dayResultStore.dayResults"
        :key="index"
        @click="handleGo(item)"
      >
        <div class="top">
          <div style="border-right: silver solid 1px">
            <h3>{{ item.startDate }}</h3>
          </div>
          <div>
            <h3>{{ item.disciplineName }}</h3>
            <p>{{ item.eventUnitName }}</p>
          </div>
        </div>
        <div
          class="bottom"
          v-if="item.competitors"
        >
          <div
            v-for="(competitor,competitorIndex) in item.competitors"
            :key="competitorIndex"
            class="competitor"
          >
            <div class="nation">
              <img
                :src="'https://olympics.com/OG2024/assets/images/flags/OG2024/' + competitor.noc + '.webp'"
                alt=""
                style="height: 25px;margin-right: 10px;border: silver solid 1px;"
              >
              <p>{{ competitor.name }}</p>
            </div>
            <p>{{ competitor.results ? competitor.results.mark : '' }}</p>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useDayResultStore } from '@/stores/dayResult'
import { useCompeteDetailStore } from '@/stores/competeDetail'
import router from '@/router'
const dayResultStore = useDayResultStore()

interface Result {
  mark:string
}

interface Competitor {
  noc: string;
  name: string;
  results: Result;
}

interface Event {
  disciplineName: string;
  eventUnitName: string;
  id: string;
  disciplineCode: string;
  eventId: string;
  startDate: string;
  competitors: Competitor[];
}

const competeDetailStore = useCompeteDetailStore()
const handleGo= async (item: Event) => {
  const resp = await competeDetailStore.fetchCompeteDetail(item.disciplineCode, item.eventId)
  if (resp.code===1){
    await router.push('/detail')
  }
  else {
    alert(resp.message)
  }
}
</script>

<style scoped>
.container{
  width: 90%;
  margin: 0 auto;
  box-sizing: border-box;
}

.date{
  display: flex;
  width: 100%;
  height: 8vh;
  color: white;
  align-items: center;
  background: linear-gradient(to right, #012C70, #0092D6,#009CE0);
  margin-bottom: 3vh;
  h1{
    margin-left: 3vw;
    font-size: 3vh;
  }
}

.matches{
  width: 100%;
}

.item{
  width: 100%;
  border: silver solid 1px;
  border-radius: 10px;
  overflow: hidden;
  margin-bottom: 3vh;
}

.top{
  display: flex;
  border-bottom: silver solid 1px;
  p{
    font-size: 15px;
    margin: 5px 10px;
  }
  h3{
    margin: 5px 10px;
  }
}

.bottom{
  width: 100%;
  box-sizing: border-box;
  padding: 0 3vw;
}

.competitor{
  display: flex;
  justify-content: space-between;
}

.nation{
  width: 60%;
  display: flex;
  justify-content: start;
  align-items: center;
  box-sizing: border-box;
  padding-left: 7vw;
}
</style>
