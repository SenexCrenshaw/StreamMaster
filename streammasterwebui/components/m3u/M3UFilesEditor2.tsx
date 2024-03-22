import { memo } from 'react';
import M3UFileDialog2 from './M3UFileDialog2';
import M3UFilesDataSelector from './M3UFilesDataSelector';

const M3UFilesEditor2 = () => (
  <div className="m3uFilesEditor flex flex-column col-12">
    <div className="flex justify-content-between align-items-center mb-1">
      <span style={{ color: 'var(--orange-color)' }}>M3U Files</span>
      <div className="flex">
        <M3UFileDialog2 />
      </div>
    </div>
    <M3UFilesDataSelector />
  </div>
);

M3UFilesEditor2.displayName = 'M3UFilesEditor2';

export interface M3UFilesEditorProperties {}

export default memo(M3UFilesEditor2);
