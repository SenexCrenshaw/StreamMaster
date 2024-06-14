import { Logger } from '@lib/common/logger';
import { useAppDispatch, useAppSelector } from '../hooks';
import { RootState } from '../store';
import { selectIsCellLoading, selectIsRowLoading, setCellLoading } from './loading';

interface UseIsCellLoadingProps {
  Entity: string;
  Id: string;
  Field: string;
}

const useIsCellLoading = ({ Entity, Id, Field }: UseIsCellLoadingProps): [boolean, (isLoading: boolean) => void] => {
  const dispatch = useAppDispatch();

  const isRowLoading = useAppSelector((state: RootState) => selectIsRowLoading(state, Entity, Id));
  const isCellLoadingInt = useAppSelector((state: RootState) => selectIsCellLoading(state, Entity, Id, Field));

  const isCellLoading = isRowLoading || isCellLoadingInt;

  const setIsCellLoading = (isLoading: boolean) => {
    Logger.debug(`Setting cell loading for ${Entity} ${Id} ${Field} to ${isLoading}`);
    dispatch(setCellLoading({ entity: Entity, field: Field, id: Id, isLoading }));
  };

  return [isCellLoading, setIsCellLoading];
};

export default useIsCellLoading;
