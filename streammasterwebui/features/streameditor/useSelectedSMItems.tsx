import { useSelectedSMChannel } from '@lib/redux/slices/selectedSMChannel';
import { useSelectedSMStream } from '@lib/redux/slices/selectedSMStream';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';

const useSelectedSMItems = () => {
  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMChannel('SMChannelDataSelector');
  const { selectedSMStream, setSelectedSMStream } = useSelectedSMStream('SMChannelDataSelector');
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems('selectSelectedSMStreamDtoItems');

  return { selectedSMChannel, selectedSMStream, selectSelectedItems, setSelectedSMChannel, setSelectedSMStream, setSelectSelectedItems };
};

export default useSelectedSMItems;
