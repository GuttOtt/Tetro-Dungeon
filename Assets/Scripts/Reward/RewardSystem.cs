using Card;
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

        [SerializeField] private Button selectButton;
        [SerializeField] private CardSelector cardSelector;
        [SerializeField] private TMP_Text rewardRemainText; //남은 보상을 표시해주는 텍스트

        [SerializeField] private SceneChanger _sceneChanger;
        [SerializeField] private GameObject _tooltip;

        [SerializeField] private RewardPanel[] blockCardSlots;
        [SerializeField] private RewardPanel[] unitConfigSlots;

        [SerializeField] private DisplayCard_UI createdCard;

        [SerializeField] private int rewardAmount = 3;
        [SerializeField] private int rewardCount = 0;

        private BlockCard _selectedBlockCard;
        private UnitConfig _selectedUnitConfig;

        private List<GameObject> generatedRewards = new List<GameObject>();
        private GameObject selectedReward = null;

        private UnitBlockDrawer _unitBlockMarker { get => cardSelector.UnitblockMarker; }


        public void DisplayReward(bool flag)
        {
            _rewardPanel.gameObject.SetActive(flag);
            _victoryText.gameObject.SetActive(flag);

            _victoryText.text = flag ? "Player Wins!" : "Player Loses...";

            rewardRemainText.text = "남은 보상: " + (rewardAmount - rewardCount).ToString();

            if (flag)
            {
                GenerateBlockCards();
                GenerateUnitConfigs();

                _selectedBlockCard = blockCardSlots[0].BlockCard;
                _selectedUnitConfig = unitConfigSlots[0].UnitConfig;

                createdCard.Init(_selectedUnitConfig, _selectedBlockCard);
            }
        }

        private void GenerateBlockCards() {
            for (int i = 0; i < blockCardSlots.Length; i++) {
                GameObject rewardObject = new GameObject("reward");

                rewardObject.AddComponent<BlockCard>();
                BlockCard blockCard = rewardObject.GetComponent<BlockCard>();
                blockCard.Init(GetRandomBlockCard());

                // BlockCard에 툴팁 프리팹 설정
                GameObject tooltipInstance = Instantiate(_tooltip, blockCard.transform); // 툴팁 프리팹 생성
                tooltipInstance.transform.SetParent(blockCard.transform, false);  // 부모를 설정하고 로컬 포지션 유지
                tooltipInstance.transform.localPosition = new Vector3(0f, 200f, 1f);

                tooltipInstance.SetActive(false); // 기본적으로 비활성화
                blockCard.TooltipPrefab = tooltipInstance; // BlockCard에 툴팁 할당


                var cells = _unitBlockMarker.DrawBlock(blockCard.Polyomino, blockCardSlots[i].transform);

                foreach (var cell in cells) {
                    cell.transform.SetParent(blockCard.transform, false);  // block의 자식으로 설정하고 로컬 포지션 유지
                }


                rewardObject.transform.SetParent(blockCardSlots[i].transform, false);  // 부모를 설정하고 로컬 포지션 유지
                rewardObject.layer = 5; // UI

                //클릭 시 선택되도록
                blockCardSlots[i].onClick += () => SelectBlockCard(blockCard);

                //슬롯에 정보 저장
                blockCardSlots[i].BlockCard = blockCard;
            }

        }

        private void GenerateUnitConfigs() {
            for (int i = 0; i < unitConfigSlots.Length; i++) {
                GameObject rewardObject = Instantiate(unitConfigPrefab, unitConfigSlots[i].transform);

                // UnitConfig 생성 및 초기화
                UnitConfig unitConfig = GetRandomUnitConfig();
                UnitConfigUIDrawer unitDrawer = rewardObject.GetComponent<UnitConfigUIDrawer>();
                unitDrawer.Draw(unitConfig);

                //클릭 시 선택되도록
                unitConfigSlots[i].onClick += () => SelectUnitConfig(unitConfig);

                //슬롯에 정보 저장
                unitConfigSlots[i].UnitConfig = unitConfig;
            }
        }

        private void SelectBlockCard(BlockCard blockCard) {
            _selectedBlockCard = blockCard;
            createdCard.Init(_selectedUnitConfig, _selectedBlockCard);
        }

        private void SelectUnitConfig(UnitConfig unitConfig) {
            _selectedUnitConfig = unitConfig;
            createdCard.Init(_selectedUnitConfig, _selectedBlockCard);
        }

        private BlockCard GetRandomBlockCard() => Player.Instance.CreateRandomBlockCard();

        private UnitConfig GetRandomUnitConfig() => Player.Instance.GetRandomUnitConfig();

        public void OnSelectButtonClicked()
        {

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

                //Check Reward Count
                rewardCount++;
                
                if (rewardCount == rewardAmount) {
                    LoadNextStage();
                }
                else {
                    DisplayReward(true);
                }
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
