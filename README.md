# Unity WebXR Sample

UnityでWebXRコンテンツをビルドするためのサンプルプロジェクトです。

## 作成環境

- **Unity**: 6000.3.4f1
- **レンダリング**: Universal Render Pipeline (URP)
- **プラットフォーム**: WebGL Build Support

## セットアップ手順

### 1. プロジェクト作成

1. Unity Hubで「New Project」
2. テンプレート: 「3D (URP)」を選択
3. プロジェクト名を設定して作成

### 2. WebXR Exportパッケージのインストール

#### 方法A: OpenUPM経由（推奨）

1. `Edit > Project Settings > Package Manager` で以下を追加:
   - **Name**: OpenUPM
   - **URL**: https://package.openupm.com
   - **Scope**: com.de-panther

2. `Window > Package Manager` から以下をインストール:
   - WebXR Export
   - WebXR Interactions

#### 方法B: Git URL経由

`Window > Package Manager > + > Add package from git URL...` で以下を追加:

```
https://github.com/De-Panther/unity-webxr-export.git?path=/Packages/webxr
https://github.com/De-Panther/unity-webxr-export.git?path=/Packages/webxr-interactions
```

### 3. WebGLテンプレートのコピー（重要）

パッケージインストール後、以下の手順でWebGLテンプレートをコピーする必要があります:

1. **`Window > WebXR > Copy WebGLTemplates`** を実行
2. **Unityを再起動**
3. 再起動後、WebGL Templateに「WebXR2020」が表示されるようになります

> この手順を行わないと、ビルド設定でWebXRテンプレートが選択できません。

### 4. シーンのセットアップ

1. Main Cameraを削除または無効化
2. `Packages/WebXR/Prefabs/WebXRCameraSet.prefab` をシーンにドラッグして配置
3. 必要に応じてオブジェクトを追加（Plane、Cubeなど）

### 5. ビルド設定

1. `File > Build Settings`
2. Platform: **WebGL** を選択 → **Switch Platform**
3. `Edit > Project Settings > Player > WebGL設定`
   - Resolution and Presentation
   - **WebGL Template: 「WebXR2020」を選択**
4. `Build Settings > Build And Run`
5. 出力フォルダを指定してビルド

## Hit Testスクリプト例

```csharp
using UnityEngine;
using WebXR;

public class SimpleHitTest : MonoBehaviour
{
    public GameObject objectToPlace;
    private WebXRManager webXRManager;

    void Start()
    {
        webXRManager = WebXRManager.Instance;
    }

    void Update()
    {
        // タッチまたはクリックで配置
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Instantiate(objectToPlace, hit.point, Quaternion.identity);
            }
        }
    }
}
```

## テスト方法

| 環境 | 方法 |
|------|------|
| PC | Chrome/Edgeで開く（WebXR Emulator拡張機能推奨） |
| スマホAR | Android Chrome でHTTPS経由でアクセス |
| VR | Quest BrowserでアクセスしてVRモード起動 |

> **注意**: WebXRはHTTPSが必要です。ローカルテストには `localhost` を使うか、HTTPS対応のサーバーにデプロイしてください。

## 参考リンク

- [unity-webxr-export](https://github.com/De-Panther/unity-webxr-export) - メインリポジトリ
- [デモページ](https://de-panther.github.io/unity-webxr-export/Build/) - 動作確認用
- [WebXR Hit Test仕様](https://www.w3.org/TR/webxr-hit-test-1/) - W3C仕様
