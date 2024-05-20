import { useFilters } from '@lib/redux/hooks/filters';
import { useQueryAdditionalFilters } from '@lib/redux/hooks/queryAdditionalFilters';
import { useQueryFilter } from '@lib/redux/hooks/queryFilter';
import { useSelectAll } from '@lib/redux/hooks/selectAll';
import { useSelectedItems } from '@lib/redux/hooks/selectedItems';
import { useShowHidden } from '@lib/redux/hooks/showHidden';
import { useSortInfo } from '@lib/redux/hooks/sortInfo';
import { ChannelGroupDto } from '@lib/smAPI/smapiTypes';

const useSelectedAndQ = (dataKey: string) => {
  const { selectedItems, setSelectedItems } = useSelectedItems<ChannelGroupDto>(dataKey);
  const { selectAll, setSelectAll } = useSelectAll(dataKey);
  const { queryFilter } = useQueryFilter(dataKey);
  const { sortInfo } = useSortInfo(dataKey);
  const { filters, setFilters } = useFilters(dataKey);
  const { queryAdditionalFilters, setQueryAdditionalFilters } = useQueryAdditionalFilters(dataKey);
  const { showHidden } = useShowHidden(dataKey);
  return {
    selectedItems,
    setSelectedItems,
    selectAll,
    queryAdditionalFilters,
    setQueryAdditionalFilters,
    setSelectAll,
    showHidden,
    queryFilter,
    sortInfo,
    filters,
    setFilters
  };
};

export default useSelectedAndQ;
