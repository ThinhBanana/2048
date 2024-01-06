using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Tile : MonoBehaviour
{


    public TileState state { get; private set; }
    public TileCell cell { get; private set; }
    public int number { get; private set; }
    public bool isLocked { get; set; }


    private Image tileBg;
    private TextMeshProUGUI tileText;


    private void Awake()
    {
        tileBg = GetComponent<Image>();
        tileText = GetComponentInChildren<TextMeshProUGUI>();
        isLocked = false;
    }

    public void SetState(TileState state, int number)
    {
        this.state = state;
        this.number = number;

        tileBg.color = state.BgColor;
        tileText.color = state.TextColor;
        tileText.text = number.ToString();
    }

    public void Spawn(TileCell cell)
    {
        // Spawn this tile into a cell
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = cell;
        this.cell.tile = this;

        transform.position = cell.transform.position;
    }

    public void MoveTo(TileCell cell)
    {
        // Remove the old cell
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        // Add the new cell
        this.cell = cell;
        this.cell.tile = this;

        // Move to new cell
        StartCoroutine(IAnimate(cell.transform.position));
    }

    public void Merge(TileCell cell)
    {
        if (this.cell != null)
        {
            this.cell.tile = null;
        }

        this.cell = null;

        isLocked = true;

        StartCoroutine(IAnimate(cell.transform.position, true));
    }

    private IEnumerator IAnimate(Vector3 to, bool isMerging = false)
    {
        float elapsed = 0f;

        Vector3 from = transform.position;

        while (elapsed < Constants.TILE_ANIMATION_DURATION)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / Constants.TILE_ANIMATION_DURATION);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (isMerging)
        {
            Destroy(gameObject);
        }
    }

    
}
