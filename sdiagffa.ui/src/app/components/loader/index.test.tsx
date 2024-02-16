import { describe, expect, it } from 'vitest';
import { render } from '@testing-library/react';
import Loader from '.'

describe('Loader component', () => {
  it('should match snapshot', () => {
    const component = render(<Loader />);
    expect(component).toMatchSnapshot();
  })
});