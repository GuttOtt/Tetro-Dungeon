using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static Unity.Collections.AllocatorManager;

namespace Assets.Scripts.Reward
{
    public class RewardSystem : MonoBehaviour
    {
        [SerializeField] private Transform _rewardPanel; // 승리 패널 UI
        [SerializeField] private TMP_Text _victoryText;
        [SerializeField] private GameObject unitConfigPrefab;
        [SerializeField] private GameObject blockCardPrefab;

        [SerializeField] private RewardPanel[] rewardSlots; // Reward 1, 2, 3 위치 (RewardDisplay의 자식들)
        [SerializeField] private Button selectButton;
        [SerializeField] private CardSelector cardSelector;

        [SerializeField] private SceneChanger _sceneChanger;
        [SerializeField] private GameObject _tooltip;

        private List<GameObject> generatedRewards = new List<GameObject>();
        private GameObject selectedReward = null;

        private UnitBlockDrawer _unitBlockMarker { get => cardSelector.UnitblockMarker; }

        public void DisplayReward(bool flag)
        {
            _rewardPanel.gameObject.SetActive(flag);
            _victoryText.gameObject.SetActive(flag);

            _victoryText.text = flag ? "Player Wins!" : "Player Loses...";

            if (flag)
            {
                GenerateRewards();
            }
        }

        private void GenerateRewards()
        {
            // 기존 보상 제거
            foreach (var reward in generatedRewards)
            {
                Destroy(reward);
            }
            generatedRewards.Clear();

            // 3개의 무작위 보상 생성
            for (int i = 0; i < rewardSlots.Length; i++)
            {
                GameObject rewardObject = new GameObject("reward");

                // 무작위로 BlockCard 혹은 UnitConfig 할당
                if (UnityEngine.Random.value > 0.5f)
                {
                    rewardObject.AddComponent<BlockCard>();
                    BlockCard blockCard = rewardObject.GetComponent<BlockCard>();
                    blockCard.Init(GetRandomBlockCard());

                    // BlockCard에 툴팁 프리팹 설정
                    GameObject tooltipInstance = Instantiate(_tooltip, blockCard.transform); // 툴팁 프리팹 생성
                    tooltipInstance.transform.SetParent(blockCard.transform, false);  // 부모를 설정하고 로컬 포지션 유지
                    tooltipInstance.transform.localPosition = new Vector3(0f, 200f, 1f);

                    tooltipInstance.SetActive(false); // 기본적으로 비활성화
                    blockCard.TooltipPrefab = tooltipInstance; // BlockCard에 툴팁 할당


                    var cells = _unitBlockMarker.DrawBlock(blockCard.Polyomino, rewardSlots[i].transform);

                    foreach (var cell in cells)
                    {
                        cell.transform.SetParent(blockCard.transform, false);  // block의 자식으로 설정하고 로컬 포지션 유지
                    }
                }
                else
                {
                    rewardObject = Instantiate(unitConfigPrefab, rewardSlots[i].transform);

                    // UnitConfig 생성 및 초기화
                    UnitConfigUIDrawer unitDrawer = rewardObject.GetComponent<UnitConfigUIDrawer>();
                    unitDrawer.Draw(GetRandomUnitConfig());
                }
                rewardObject.transform.SetParent(rewardSlots[i].transform, false);  // 부모를 설정하고 로컬 포지션 유지
                rewardObject.layer = 5; // UI

                // 리워드를 리스트에 추가
                generatedRewards.Add(rewardObject);
            }
        }

        private BlockCard GetRandomBlockCard() => Player.Instance.CreateRandomBlockCard();

        private UnitConfig GetRandomUnitConfig() => Player.Instance.GetRandomUnitConfig();

        public GameObject GetSelectedRewardItem()
        {
            foreach (RewardPanel panel in rewardSlots)
            {
                GameObject selectedItem = panel.GetSelectedItem();
                if (selectedItem != null)
                {
                    var res = selectedItem;
                    panel.Reset();
                    return res;
                }
            }

            Debug.LogWarning("No item is selected in any panel.");
            return null;
        }
        public void OnSelectButtonClicked()
        {
            selectedReward = GetSelectedRewardItem();
            if (selectedReward != null)
            {
                // 선택된 보상을 플레이어의 ExtraDeck에 추가
                if (selectedReward.GetComponent<BlockCard>() != null)
                {
                    Player.Instance.AddBlockCard(selectedReward.GetComponent<BlockCard>());
                }
                else if (selectedReward.GetComponent<UnitConfigUIDrawer>() != null)
                {
                    Player.Instance.AddUnitConfig(selectedReward.GetComponent<UnitConfigUIDrawer>().UnitConfig);
                }

                // 다음 스테이지로 이동 (구현 필요)
                LoadNextStage();
            }
        }

        private async void LoadNextStage()
        {
            DisplayReward(false);
            // 다음 스테이지 로드 로직을 구현합니다.
            Debug.Log("Next stage loading...");

            await UniTask.Delay(TimeSpan.FromSeconds(3f));

            _sceneChanger.LoadStageScene();
        }
    }
}
