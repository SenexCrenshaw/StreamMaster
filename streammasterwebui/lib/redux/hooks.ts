import { type TypedUseSelectorHook, useDispatch, useSelector } from 'react-redux';
import { AppDispatch, type RootState } from './store';

// Use throughout your app instead of plain `useDispatch` and `useSelector`
interface DispatchFunction {
  (): AppDispatch;
}
export const useAppDispatch: DispatchFunction = useDispatch;
export const useAppSelector: TypedUseSelectorHook<RootState> = useSelector;
