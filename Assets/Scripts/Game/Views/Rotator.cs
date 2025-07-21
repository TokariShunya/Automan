using UnityEngine;
using R3;
using R3.Triggers;

namespace Automan.Game.View
{
    /// <summary>
    /// 回転するオブジェクト
    /// </summary>
    public sealed class Rotator : MonoBehaviour
    {
        [SerializeField] private Vector3 _eulerAngularVelocity;

        private void Start()
        {
            this.UpdateAsObservable()
                .Subscribe(_ =>
                {
                    transform.Rotate(_eulerAngularVelocity * Time.deltaTime, Space.Self);
                })
                .RegisterTo(destroyCancellationToken);
        }
    }
}