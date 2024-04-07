import { useSelectedSMChannel } from '@lib/redux/slices/selectedSMChannel';
import { useSelectedSMStream } from '@lib/redux/slices/selectedSMStream';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';
import { SMStreamDto } from '@lib/smAPI/smapiTypes';

const useSelectedSMItems = () => {
  const { selectSelectedItems: expandedRows, setSelectSelectedItems: setExpandedRows } = useSelectedItems<SMStreamDto>('selectExpandedRows');
  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMChannel('SMChannelDataSelector');
  const { selectedSMStream, setSelectedSMStream } = useSelectedSMStream('SMChannelDataSelector');
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems('selectSelectedSMStreamDtoItems');

  return {
    selectedSMChannel,
    selectedSMStream,
    selectSelectedItems,
    expandedRows,
    setExpandedRows,
    setSelectedSMChannel,
    setSelectedSMStream,
    setSelectSelectedItems
  };
};

export default useSelectedSMItems;
