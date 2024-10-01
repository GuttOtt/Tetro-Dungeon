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

        private RewardPanel _selectedBlockCardSlot;
        private RewardPanel _selectedUnitConfigSlot;

        private BlockCard _selectedBlockCard { get => _selectedBlockCardSlot?.BlockCard; }
        private UnitConfig _selectedUnitConfig { get => _selectedUnitConfigSlot?.UnitConfig; }

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

                SelectBlockCard(blockCardSlots[0]);
                SelectUnitConfig(unitConfigSlots[0]);
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

                //클릭 액션 등록
                RewardPanel blockCardSlot = blockCardSlots[i];
                blockCardSlot.onClick += () => SelectBlockCard(blockCardSlot);

                //슬롯에 정보 저장
                blockCardSlots[i].BlockCard = blockCard;
            }

        }

        private void GenerateUnitConfigs() {
            for (int i = 0; i < unitConfigSlots.Length; i++) {
                GameObject rewardObject = Instantiate(unitConfigPrefab, unitConfigSlots[i].transform);
                rewardObject.name = "reward";

                // UnitConfig 생성 및 초기화
                UnitConfig unitConfig = GetRandomUnitConfig();
                UnitConfigUIDrawer unitDrawer = rewardObject.GetComponent<UnitConfigUIDrawer>();
                unitDrawer.Draw(unitConfig);

                //클릭 액션 등록
                RewardPanel unitConfigSlot = unitConfigSlots[i];
                unitConfigSlot.onClick += () => SelectUnitConfig(unitConfigSlot);

                //슬롯에 정보 저장
                unitConfigSlots[i].UnitConfig = unitConfig;
            }
        }

        public void RerollBlockCards() {
            //리롤할 돈이 되는지 검사

            ResetBlockCardSlots();
            GenerateBlockCards();
            SelectBlockCard(blockCardSlots[0]);
        }

        public void RerollUnitConfigs() {
            ResetUnitConfigSlots();
            GenerateUnitConfigs();
            SelectUnitConfig(unitConfigSlots[0]);
        }


        private void SelectBlockCard(RewardPanel blockCardSlot) {
            //색상 변경
            if (_selectedBlockCardSlot != null) {
                _selectedBlockCardSlot.ChangeColor(Color.white);
            }
            _selectedBlockCardSlot = blockCardSlot;
            _selectedBlockCardSlot.ChangeColor(new Color(0.5f, 1f, 0.5f, 0.5f));

            UpdateCreatedCard();
        }

        private void SelectUnitConfig(RewardPanel unitConfigSlot) {
            //색상 변경
            if (_selectedUnitConfigSlot != null) {
                _selectedUnitConfigSlot.ChangeColor(Color.white);
            }
            _selectedUnitConfigSlot = unitConfigSlot;
            _selectedUnitConfigSlot.ChangeColor(new Color(0.5f, 1f, 0.5f, 0.5f));

            UpdateCreatedCard();
        }

        private void UpdateCreatedCard() {
            if (_selectedBlockCard == null || _selectedUnitConfig == null)
                return;

            createdCard.Init(_selectedUnitConfig, _selectedBlockCard);
        }

        private BlockCard GetRandomBlockCard() => Player.Instance.CreateRandomBlockCard();

        private UnitConfig GetRandomUnitConfig() => Player.Instance.GetRandomUnitConfig();

        public void OnSelectButtonClicked()
        {
            //완성된 카드를 Player 클래스에 추가
            var cardData = new CardData(_selectedUnitConfig, _selectedBlockCard);
            Player.Instance.ExtraDeck.Add(cardData);
            Debug.Log("Card added to ExtraDeck");

            //Check Reward Count
            rewardCount++;
                
            if (rewardCount == rewardAmount) {
                LoadNextStage();
            }
            else {
                ResetSlots();
                DisplayReward(true);
            }
          
        }

        #region Reset Slots
        private void ResetSlots() {
            ResetBlockCardSlots();
            ResetUnitConfigSlots();
            //item의 Init과 삭제 방식 수정해야함
            //Slot 자체에 Init 메소드를 넣을 것.
            //필요하다면 RewardPanel 클래스를 상속을 통해 두 종류로 구분할 것.
        }

        private void ResetBlockCardSlots() {
            foreach (var slot in blockCardSlots) {
                slot.Reset();
                GameObject item = slot.transform.Find("reward").gameObject;
                Destroy(item);
            }
        }

        private void ResetUnitConfigSlots() {
            foreach (var slot in unitConfigSlots) {
                slot.Reset();
                GameObject item = slot.transform.Find("reward").gameObject;
                Destroy(item);
            }
        }
        #endregion

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
