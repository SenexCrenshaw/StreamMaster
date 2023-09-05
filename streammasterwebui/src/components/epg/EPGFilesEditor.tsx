import '../../styles/EPGFilesEditor.css';
import EPGFilesDataSelector from './EPGFilesDataSelector';
import { memo } from 'react';
import { type EpgFilesDto } from '../../store/iptvApi';
import EPGFileDialog from './EPGFileDialog';

const EPGFilesEditor = () => {
  return (
    <div className='m3uFilesEditor flex flex-column col-12 flex-shrink-0 '>
      <div className='flex justify-content-between align-items-center mb-1'>
        <span className='m-0 p-0 gap-1' style={{ color: '#FE7600' }}>EPG Files</span>
        <div className='m-0 p-0 flex gap-1'>
          <EPGFileDialog />
        </div>
      </div>
      <EPGFilesDataSelector />

    </div>

  );
}

EPGFilesEditor.displayName = 'EPGFilesEditor';
export type EPGFilesEditorProps = {
  onClick?: (e: EpgFilesDto) => void;
  value?: EpgFilesDto | undefined;
};
export default memo(EPGFilesEditor);
