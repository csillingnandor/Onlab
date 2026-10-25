import type { ComponentPropsWithRef, ReactNode } from 'react';
import { Button as BootstrapButton, type ButtonProps as BootstrapButtonProps } from 'react-bootstrap';

export type ButtonProps = Omit<BootstrapButtonProps, 'as'> & Omit<ComponentPropsWithRef<'button'>, keyof BootstrapButtonProps>;

// A react-bootstrap Button, <button> elemre rögzített típussal. Az eredeti polimorf (as="...") típusa a
// JSX elemek közül választ; a 3D nézet (@react-three/fiber) több száz elemmel bővíti a JSX-et, amitől ez a
// típus túl összetett lesz (TS2590). A <button>-ként használt gombokhoz ezt a változatot használjuk.
const Button = BootstrapButton as unknown as (props: ButtonProps) => ReactNode;

export default Button;
