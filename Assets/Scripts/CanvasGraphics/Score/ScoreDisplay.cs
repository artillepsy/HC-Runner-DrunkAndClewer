﻿using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using Finish;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace CanvasGraphics.Score
{
    public class ScoreDisplay : MonoBehaviour
    {
        [SerializeField] private float borderPosX = 225f;
        [SerializeField] private RectTransform pointer;
        [SerializeField] private Image pointerImg;
        [SerializeField] private GameObject scoreGO;
        [SerializeField] private TextMeshProUGUI stageLabel;
        [SerializeField] private float changeTime = 0.2f;
        
        [Space]
        [SerializeField] private List<ProgressStage> states;
        private ScoreChanger _scoreChanger;
        private Coroutine _changePosCO;

        private void Start()
        {
            _scoreChanger = FindObjectOfType<ScoreChanger>();
            ScoreChanger.OnScoreChange.AddListener((score) => StartCoroutine(UpdateVisualsCO(score)));
            FinishLine.OnReachedFinish.AddListener(() => scoreGO.SetActive(false));
            StartCoroutine(UpdateVisualsCO(0));
        }

        private IEnumerator UpdateVisualsCO(int score)
        {
            var currentState = GetCurrentStage(score);
            if (!currentState) yield break;
            
            stageLabel.text = currentState.Name;
            pointerImg.color = currentState.LabelColor;

            //progressBar.color = currentState.SliderColor;
            if (_changePosCO != null)
            {
                StopCoroutine(_changePosCO);
                _changePosCO = null;
            }
            _changePosCO = StartCoroutine(SetPointerPosXCO(score));
        }
        
        private ProgressStage GetCurrentStage(int currentScore)
        {
            foreach (var state in states)
            {
                if (currentScore < state.LeftPointsBorder || currentScore > state.RightPointsBorder) continue;
                return state;
            }
            return null;
        }

        private IEnumerator SetPointerPosXCO(int currentScore)
        {
            var startPos = pointer.anchoredPosition;
            var endX = (float) currentScore / _scoreChanger.MaxScore * borderPosX;
            var endPos = new Vector2(endX, startPos.y);
            
            var maxSpeed = (endPos.x - startPos.x) / changeTime;
            var time = 0f;
            Vector2 velocity = Vector2.zero;

            while (time < changeTime)
            {
                pointer.anchoredPosition = Vector2.SmoothDamp(
                    pointer.anchoredPosition, 
                    endPos,
                    ref velocity,
                    changeTime);
                time += Time.deltaTime;
                yield return null;
            }
            _changePosCO = null;
        }
    }
}