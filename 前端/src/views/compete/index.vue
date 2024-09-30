<template>
  <div class="container">
    <div class="header">
      <h1>1/4决赛</h1>
      <h1>半决赛</h1>
      <h1>决赛</h1>
    </div>
    <div class="content">
<!--       1/4决赛-->
      <div class="col">
        <div
          class="item"
          v-for="(item,index) in competitionStore.information.qFinal.allMatch"
          :key="index"
        >
          <div class="competitor">
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
          <div class="competitor">
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
<!--      连线-->
      <div class="col-gap">
        <div class="line-box">
          <div class="line-left"></div>
          <div class="line-right">
            <div></div>
            <div></div>
          </div>
        </div>
        <div class="line-box">
          <div class="line-left"></div>
          <div class="line-right">
            <div></div>
            <div></div>
          </div>
        </div>
      </div>
<!--      半决赛-->
      <div class="col">
        <div
          class="item"
          v-for="(item,index) in competitionStore.information.halfFinal.allMatch"
          :key="index"
        >
          <div class="competitor">
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
          <div class="competitor">
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
<!--      连线-->
      <div class="col-gap">
        <div class="line-box" style="height: 50%">
          <div class="line-left"></div>
          <div class="line-right">
            <div></div>
            <div></div>
          </div>
        </div>
      </div>
<!--      决赛-->
      <div class="col-final">
        <div
          class="item"
          v-for="(item,index) in competitionStore.information.final.allMatch"
          :key="index"
        >
          <div style="border-bottom: silver solid 1px;">
            <h3>{{ competitionStore.information.final.description }}</h3>
          </div>
          <div class="competitor">
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
          <div class="competitor">
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
        <div v-if="competitionStore.information.final2.allMatch.length > 0">
          <div
            class="item"
            v-for="(item,index) in competitionStore.information.final2.allMatch"
            :key="index"
          >
            <div style="border-bottom: silver solid 1px;">
              <h3>{{ competitionStore.information.final2.description }}</h3>
            </div>
            <div class="competitor">
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
            <div class="competitor">
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
  </div>
</template>

<script setup lang="ts">
import { useCompetitionStore } from '@/stores/competition'
// import { onMounted } from 'vue'

const competitionStore = useCompetitionStore()

// onMounted(async () => {
//   await competitionStore.fetchProject()
//   await competitionStore.fetchTypes(competitionStore.selected.firstname)
//   await competitionStore.fetchInfo(competitionStore.selected.type.id)
// })
</script>

<style scoped>
.container{
  width: 90%;
  min-width: 1024px;
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
}

.col{
  width: 30%;
  height: 70vh;
  min-width: 265px;
  min-height: 528px;
  display: flex;
  flex-direction: column;
  justify-content: space-around;
}

.col-final{
  width: 30%;
  height: 70vh;
  display: flex;
  flex-direction: column;
  justify-content: end;
  position: relative;
}

.col-final>div:first-child{
  width: 100%;
  position: absolute;
  top: 50%;
  left: 50%;
  transform: translate(-50%, -50%);
}

.item{
  display: flex;
  flex-direction: column;
  border: silver solid 1px;
  border-radius: 10px;
  h3{
    margin: 5px 15px;
  }
}

.nation{
  display: flex;
  justify-content: start;
  align-items: center;
  box-sizing: border-box;
  padding-left: 1vw;
}

.competitor{
  display: flex;
  justify-content: space-between;
  box-sizing: border-box;
  margin: 5px 10px;
}

.col-gap{
  width: 5%;
  height: 70vh;
  min-height: 528px;
  display: flex;
  flex-direction: column;
  justify-content: space-around;
}

.line-box{
  width: 100%;
  height: 27%;
  display: flex;
  .line-left{
    width: 50%;
    height: 100%;
    border-top: silver solid 2px;
    border-right: silver solid 2px;
    border-bottom: silver solid 2px;
  }
  .line-right{
    width: 50%;
    height: 100%;
  }
  .line-right>div:first-child{
    width: 100%;
    height: 50%;
    border-bottom: silver solid 2px;
  }
}
</style>
