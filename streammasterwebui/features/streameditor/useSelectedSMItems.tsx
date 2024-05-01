import { useSelectedSMChannel } from '@lib/redux/slices/selectedSMChannel';
import { useSelectedSMStream } from '@lib/redux/slices/selectedSMStream';
import { useSelectedItems } from '@lib/redux/slices/useSelectedItemsSlice';

const useSelectedSMItems = (key?: string) => {
  const { selectedSMChannel, setSelectedSMChannel } = useSelectedSMChannel(key ?? 'SMChannelDataSelector');
  const { selectedSMStream, setSelectedSMStream } = useSelectedSMStream(key ?? 'SMChannelDataSelector');
  const { selectSelectedItems, setSelectSelectedItems } = useSelectedItems(key ?? 'SMChannelDataSelector');

  return {
    selectedSMChannel,
    selectedSMStream,
    selectSelectedItems,
    setSelectedSMChannel,
    setSelectedSMStream,
    setSelectSelectedItems
  };
};

export default useSelectedSMItems;
