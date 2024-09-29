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
      <div class="col">

      </div>
      <div class="col"></div>
      <div class="col"></div>
    </div>
  </div>
</template>

<script setup lang="ts">
import { useCompetitionStore } from '@/stores/competition'
import { onMounted } from 'vue'

const competitionStore = useCompetitionStore()

onMounted(async () => {
  await competitionStore.fetchProject()
  await competitionStore.fetchTypes(competitionStore.selected.firstname)
  await competitionStore.fetchInfo(competitionStore.selected.type.id)
  console.log('competitionStore.projects',competitionStore.projects)
  console.log('competitionStore.types',competitionStore.types)
})
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
