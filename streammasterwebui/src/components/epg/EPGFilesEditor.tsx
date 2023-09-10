import { memo } from 'react';
import { type EpgFileDto } from '../../store/iptvApi';
import '../../styles/EPGFilesEditor.css';
import EPGFileDialog from './EPGFileDialog';
import EPGFilesDataSelector from './EPGFilesDataSelector';

const EPGFilesEditor = () => {
  return (
    <div className='m3uFilesEditor flex flex-column col-12'>
      <div className='flex justify-content-between align-items-center mb-1'>
        <span style={{ color: 'var(--orange-color)' }}>EPG Files</span>
        <div className='flex'>
          <EPGFileDialog />
        </div>
      </div>
      <EPGFilesDataSelector />

    </div>

  );
}

EPGFilesEditor.displayName = 'EPGFilesEditor';
export type EPGFilesEditorProps = {
  onClick?: (e: EpgFileDto) => void;
  value?: EpgFileDto | undefined;
};
export default memo(EPGFilesEditor);
