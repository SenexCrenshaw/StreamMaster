import { EpgFileDto } from '@lib/iptvApi';
import { memo } from 'react';
import EPGFileDialog from './EPGFileDialog';
import EPGFilesDataSelector from './EPGFilesDataSelector';

const EPGFilesEditor = () => (
  <div className="m3uFilesEditor flex flex-column col-12">
    <div className="flex justify-content-between align-items-center mb-1">
      <span style={{ color: 'var(--orange-color)' }}>EPG Files</span>
      <div className="flex">
        <EPGFileDialog />
      </div>
    </div>
    <EPGFilesDataSelector />
  </div>
);

EPGFilesEditor.displayName = 'EPGFilesEditor';
export interface EPGFilesEditorProperties {
  onClick?: (e: EpgFileDto) => void;
  value?: EpgFileDto | undefined;
}
export default memo(EPGFilesEditor);
