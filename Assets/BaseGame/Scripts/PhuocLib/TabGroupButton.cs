using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

namespace Pextension
{

    public class TabGroupButton : MonoBehaviour
    {
        [SerializeField] public Button[] m_Buttons;
        [SerializeField] private Transform m_ImageSelect;

        [Header("Tween Img Select")]
        [SerializeField] private TweenTransition m_ImgSelectScaleUpTween;
        [SerializeField] private TweenTransition m_ImgSelectScaleDownTrasition;

        [Header("Tween Img Icon")]
        [Header("Tween Img Icon On Select")]
        [SerializeField] private List<TweenTransition> m_ImgIconScaleUpTrasitionList;
        [SerializeField] private List<TweenTransition> m_ImgIconMoveUpTrasitionList;
        [Header("Tween Img Icon On Deselect")]
        [SerializeField] private List<TweenTransition> m_ImgIconMoveDownTrasitionList;
        [SerializeField] private List<TweenTransition> m_ImgIconScaleDownTrasitionList;

        [Header("Tween Txt")]
        [SerializeField] private List<TweenTransition> m_TxtScaleupTrasition;
        [SerializeField] private List<TweenTransition> m_TxtScaleDownTrasition;

        [SerializeField] private int m_LastIndexSelected = -1;
        private UnityAction<Enum> m_ClickButton;


        private void Awake()
        {
            //m_ImgSelectMoveTrasition.Init();
            m_ImgSelectScaleUpTween?.Init();
            m_ImgSelectScaleDownTrasition?.Init();
            m_ImgIconScaleUpTrasitionList?.ForEach(t => t.Init());
            m_ImgIconMoveUpTrasitionList?.ForEach(t => t.Init());
            m_ImgIconMoveDownTrasitionList?.ForEach(t => t.Init());
            m_ImgIconScaleDownTrasitionList?.ForEach(t => t.Init());
            m_TxtScaleupTrasition?.ForEach(t => t.Init());
            m_TxtScaleDownTrasition?.ForEach(t => t.Init());

        }
        public void Setup<T>(UnityAction<T> actionCallback) where T : Enum
        {
            m_ClickButton = (Enum type) => actionCallback((T)type);
            int count = m_Buttons.Length;
            for (int i = 0; i < count; i++)
            {
                Button button = m_Buttons[i];
                Enum typeButton = (Enum)Enum.GetValues(typeof(T)).GetValue(i);
                button.onClick.AddListener(() => OnClickButton(typeButton));
            }
        }
        public void OnClickButton(Enum type)
        {
            // m_ImageSelect.DOKill();
            var trans = m_Buttons[Convert.ToInt32(type)].transform;
            //Vector3 pos = m_Buttons[1].transform.position;
            // m_ImageSelect.DOMoveX(pos.x, 0.15f);
            m_ImageSelect.transform.SetParent(trans);
            // m_ImageSelect.transform.localPosition = Vector3.zero;
            m_ImageSelect.transform.SetAsFirstSibling();
            if (m_LastIndexSelected != -1 && m_ImgIconMoveDownTrasitionList.Count > 0)
            {
                m_ImgIconMoveDownTrasitionList[m_LastIndexSelected].Play();
                m_ImgIconScaleDownTrasitionList[m_LastIndexSelected].Play();
                m_TxtScaleDownTrasition[m_LastIndexSelected].Play();
            }
            m_LastIndexSelected = Convert.ToInt32(type);
            if (m_ImgSelectScaleUpTween != null)
                m_ImgSelectScaleUpTween.Play();
            //m_ImgSelectMoveTrasition.Play();
            m_ImageSelect.DOLocalMove(Vector3.zero, 0.25f).OnComplete
                (
                () =>
                {
                    if (m_ImgSelectScaleDownTrasition != null)
                    {
                        m_ImgSelectScaleDownTrasition?.Play();
                        m_ImgIconScaleUpTrasitionList[m_LastIndexSelected]?.Play();
                        m_ImgIconMoveUpTrasitionList[m_LastIndexSelected]?.Play();
                        m_TxtScaleupTrasition[m_LastIndexSelected]?.Play();
                    }
                }
                );
            // Debug.Log(m_ImageSelect.transform.position + " " + pos);
            m_ClickButton?.Invoke(type);
        }
    }

}